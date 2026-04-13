using AutoMapper;
using Google.Apis.Auth;
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

        if (foundAccessToken is not null && !foundAccessToken.IsRevoked)
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

    public async ValueTask<(AccessToken accessToken, RefreshToken refreshToken)> SignInAsync(
        SignInDetails signInDetails,
        CancellationToken cancellationToken = default)
    {
        // ─── Email / Password Sign-In ─────────────────────────────────────────────
        var foundUser = await userService
            .GetByUsernameOrEmailAddressAsync(signInDetails.UsernameOrEmail, cancellationToken: cancellationToken);

        // User bazada yo'q → register bo'lishi kerakligini aytyapmiz
        if (foundUser is null)
            throw new AuthenticationException(
                "No account found with this email. Please register first.");

        // User boshqa provider bilan register bo'lgan (masalan Google)
        if (foundUser.AuthProvider != AuthProvider.Email)
            throw new AuthenticationException(
                $"This account was registered via {foundUser.AuthProvider}. Please sign in with {foundUser.AuthProvider}.");

        // Password tekshiruvi
        if (string.IsNullOrEmpty(signInDetails.Password) ||
            !passwordHasherService.ValidatePassword(signInDetails.Password, foundUser.UserCredentials.PasswordHash))
            throw new AuthenticationException(
                "Sign in details are invalid, please check your info is correct!");

        // Email verification tekshiruvi
        if (!foundUser.IsEmailAddressVerified)
            throw new AuthenticationException("Email address is not verified.");

        return await CreateTokens(foundUser, cancellationToken);
    }

    public async ValueTask<(AccessToken accessToken, RefreshToken refreshToken)> SignInWithGoogleAsync(GoogleSignInRequest idToken, CancellationToken cancellationToken = default)
    {
        var payload = await VerifyGoogleTokenService.VerifyGoogleToken(idToken.GoogleIdToken);

        if (payload is null || string.IsNullOrEmpty(payload.Email))
            throw new AuthenticationException("Invalid Google ID token.");

        // Google orqali: user yo'q bo'lsa — register qilib token qaytaramiz
        return await SignUpWithGoogleAsync(
            new GoogleSignInRequest { GoogleIdToken = idToken.GoogleIdToken},
            cancellationToken);
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

    public async ValueTask<(AccessToken accessToken, RefreshToken refreshToken)> SignUpWithGoogleAsync(GoogleSignInRequest idToken, CancellationToken cancellationToken = default)
    {
        GoogleJsonWebSignature.Payload? payload = await VerifyGoogleTokenService.VerifyGoogleToken(idToken.GoogleIdToken);
        if (payload is not null && !string.IsNullOrEmpty(payload.Email))
        {
            var foundUserId = await userService.GetByUsernameOrEmailAddressAsync(payload.Email, true, cancellationToken);
            if (foundUserId is not null)
            {
                return await CreateTokens(foundUserId, cancellationToken);
            }
            var signupDetails = new SignUpDetails
            {
                FirstName = payload?.GivenName ?? string.Empty,
                LastName = payload?.FamilyName ?? string.Empty,
                EmailAddress = payload?.Email ?? string.Empty,
                AuthProvider = AuthProvider.Google,
                AutoGeneratePassword = true,
                Age = 0,
                UserName = payload?.Email.Split("@")[0] ?? string.Empty
            };
            var user = mapper.Map<User>(signupDetails);

            var password = signupDetails.AutoGeneratePassword
                ? passwordGeneratorService.GeneratePassword()
                : passwordGeneratorService.GetValidatePassword(signupDetails.Password!, user);

            user.UserCredentials = new UserCredentials
            {
                PasswordHash = passwordHasherService.HashPassword(password)
            };

            // OAuth provider (Google, Apple, etc.) orqali bo'lsa email verification skip
            var skipEmailVerification = signupDetails.AuthProvider != AuthProvider.Email;

            var createdUser = await accountService.CreateUserAsync(user, skipEmailVerification, cancellationToken);

            await roleProcessingService.GrandRoleBySystemAsync(createdUser.Id, RoleType.Guest, cancellationToken);

            // TODO : add other validation logic
            return await CreateTokens(createdUser, cancellationToken);
        }
        else
        {
            throw new AuthenticationException("Invalid Google ID token.");
        }
    }

    private async Task<(AccessToken AccessToken, RefreshToken RefreshToken)> CreateTokens(User user, CancellationToken cancellationToken = default)
    {
        var accessToken = identitySecurityTokenGeneratorService.GenerateAccessToken(user);

        var refreshToken = identitySecurityTokenGeneratorService.GenerateRefreshTokenAsync(user);

        return (await identitySecurityTokenService.CreateAccessTokenAsync(accessToken, cancellationToken: cancellationToken),
            await identitySecurityTokenService.CreateRefreshTokenAsync(refreshToken, cancellation: cancellationToken));
    }
}
