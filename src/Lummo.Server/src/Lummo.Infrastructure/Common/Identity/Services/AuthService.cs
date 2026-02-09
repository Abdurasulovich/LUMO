using AutoMapper;
using Lummo.Application.Common.Identity.Models;
using Lummo.Application.Common.Identity.Services.Interfaces;
using Lummo.Domain.Brokers;
using Lummo.Domain.Entities;
using Lummo.Domain.Enums;
using Lummo.Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Abstractions;
using System.Security.Authentication;

namespace Lummo.Infrastructure.Common.Identity.Services;

public class AuthService(
    IMapper mapper,
    IUserService userService,
    IAccountService accountService,
    IRoleProcessingService roleProcessingService,
    IIdentitySecurityTokenService identitySecurityTokenService,
    IPasswordHasherService passwordHasherService,
    IPasswordGeneratorService passwordGeneratorService,
    IIdentitySecurityTokenGeneratorService identitySecurityTokenGeneratorService,
    IRequestUserContextProvider requestUserContextProvider
) : IAuthService
{
    public async ValueTask<bool> GrandRoleAsync(Guid userId, string roleType, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse(roleType, out RoleType roleValue))
            throw new InvalidOperationException("Invalid role type provided.");

        var grandRoleTask = () => roleProcessingService.GrandRoleAsync(
            userId,
            roleValue,
            requestUserContextProvider.GetUserRole(),
            cancellationToken
            );
        var grandRoleValue = await grandRoleTask.GetValueAsync();

        if (grandRoleValue is { IsSuccess: false, Exception: not null })
            throw grandRoleValue.Exception;

        // TODO : Send role granted notification

        return true;
    }

    public async ValueTask<AccessToken> RefreshTokenAsync(string refreshTokenValue, CancellationToken cancellationToken = default)
    {
        var accessTokenValue = requestUserContextProvider.GetAccessToken();

        if (string.IsNullOrWhiteSpace(refreshTokenValue))
            throw new ArgumentException("Invalid identity security token value.", nameof(refreshTokenValue));

        if (string.IsNullOrWhiteSpace(accessTokenValue))
            throw new InvalidOperationException("Invalid identity security token value.");

        //Check refresh token and access token
        var refreshToken = await identitySecurityTokenService.GetRefreshTokenByValueAsync(refreshTokenValue, cancellationToken)
        ?? throw new AuthenticationException("Please login again.");

        var accessToken = identitySecurityTokenGeneratorService.GetAccessToken(accessTokenValue);
        if (!accessToken.HasValue)
        {
            await identitySecurityTokenService.RemoveRefreshTokenAsync(refreshTokenValue, cancellationToken);
            throw new InvalidOperationException("Invalid identity security token value.");
        }

        var foundAccessToken = await identitySecurityTokenService.GetAccessTokenByIdAsync(accessToken.Value.AccessToken.Id,
            cancellationToken);

        //Remove refresh token and access token if user id is not same
        if (refreshToken.UserId != accessToken.Value.AccessToken.UserId)
        {
            await identitySecurityTokenService.RemoveRefreshTokenAsync(refreshTokenValue, cancellationToken);
            if (foundAccessToken is not null)
                await identitySecurityTokenService.RevokeAccessTokenAsync(accessToken.Value.AccessToken.Id, cancellationToken);

            throw new AuthenticationException("Please login again.");
        }

        var foundUser =
            await userService
            .Get(user => user.Id == accessToken.Value.AccessToken.UserId, true)
            .Include(user => user.Roles)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException();

        if(foundAccessToken is not null && !foundAccessToken.IsRevoked)
        {
            if (!foundAccessToken.IsRevoked)
                return foundAccessToken;
            await identitySecurityTokenService.RemoveAccessTokenAsync(accessToken.Value.AccessToken.Id, cancellationToken);
        }

        var newAccessToken = identitySecurityTokenGeneratorService.GenerateAccessToken(foundUser);

        return await identitySecurityTokenService.CreateAccessTokenAsync(newAccessToken, cancellationToken: cancellationToken);
    }

    public async ValueTask<bool> RevokeRoleAsync(Guid userId, string roleType, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse(roleType, out RoleType roleValue))
            throw new InvalidOperationException("Invalid role type provided");

        var revokeRoleTask = () => roleProcessingService.RevokeRoleAsync(
            userId,
            roleValue,
            requestUserContextProvider.GetUserRole(),
            cancellationToken
        );
        var grandRoleValue = await revokeRoleTask.GetValueAsync();

        if (grandRoleValue is { IsSuccess: false, Exception: not null })
            throw grandRoleValue.Exception;

        // TODO : Send role revoked notification

        return true;
    }

    public async ValueTask<(AccessToken accessToken, RefreshToken refreshToken)> SignInAsync(SignInDetails signInDetails, CancellationToken cancellationToken = default)
    {
        var foundUser = await userService.GetByUsernameOrEmailAddressAsync(signInDetails.UsernameOrEmail, cancellationToken: cancellationToken)
        ?? throw new AuthenticationException("Sign in details are invalid, please check the your info is correct!");

        // User qaysi provider bilan register bo'lgan bo'lsa, shu provider bilan sign in qilishi kerak
        if (foundUser.AuthProvider != signInDetails.AuthProvider)
            throw new AuthenticationException($"Please sign in with {foundUser.AuthProvider}.");

        // OAuth provider (Google, Apple, etc.) uchun password tekshirilmaydi
        if (signInDetails.AuthProvider == AuthProvider.Email)
        {
            if (string.IsNullOrEmpty(signInDetails.Password) ||
                !passwordHasherService.ValidatePassword(signInDetails.Password, foundUser.UserCredentials.PasswordHash))
                throw new AuthenticationException("Sign in details are invalid, please check the your info is correct!");
        }

        if (!foundUser.IsEmailAddressVerified)
            throw new AuthenticationException("Email address is not verified.");

        return await CreateTokens(foundUser, cancellationToken);
    }

    public async ValueTask<bool> SignUpAsync(SignUpDetails signUpDetails, CancellationToken cancellationToken = default)
    {
        var foundUserId = await userService.GetByUsernameOrEmailAddressAsync(signUpDetails.EmailAddress, true, cancellationToken);
        if (foundUserId is not null)
            throw new InvalidOperationException("User with this email address already exists.");

        var user = mapper.Map<User>(signUpDetails);

        var password = signUpDetails.AutoGeneratePassword
            ? passwordGeneratorService.GeneratePassword()
            : passwordGeneratorService.GetValidatePassword(signUpDetails.Password!, user);

        user.UserCredentials = new UserCredentials
        {
            PasswordHash = passwordHasherService.HashPassword(password)
        };

        // OAuth provider (Google, Apple, etc.) orqali bo'lsa email verification skip
        var skipEmailVerification = signUpDetails.AuthProvider != AuthProvider.Email;

        var createdUser = await accountService.CreateUserAsync(user, skipEmailVerification, cancellationToken);

        await roleProcessingService.GrandRoleBySystemAsync(createdUser.Id, RoleType.Guest, cancellationToken);

        // TODO : add other validation logic
        return true;
    }


    private async Task<(AccessToken AccessToken, RefreshToken RefreshToken)> CreateTokens(User user, CancellationToken cancellationToken = default)
    {
        var accessToken = identitySecurityTokenGeneratorService.GenerateAccessToken(user);

        var refreshToken = identitySecurityTokenGeneratorService.GenerateRefreshTokenAsync(user);

        return (await identitySecurityTokenService.CreateAccessTokenAsync(accessToken, cancellationToken: cancellationToken),
            await identitySecurityTokenService.CreateRefreshTokenAsync(refreshToken, cancellation: cancellationToken));
    }
}
