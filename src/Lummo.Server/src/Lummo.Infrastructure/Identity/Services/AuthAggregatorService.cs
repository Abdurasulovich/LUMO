using AutoMapper;
using Lummo.Application.Common.Identity.Models;
using Lummo.Application.Common.Identity.Services.Interfaces;
using Lummo.Domain.Entities;

namespace Lummo.Infrastructure.Identity.Services;

public class AuthAggregatorService(
    IMapper mapper,
    IPasswordGeneratorService passwordGeneratorService,
    IPasswordHasherService passwordHasherService,
    IAccountAggregatorService accountAggregatorService,
    IUserService userService,
    ) : IAuthAggregatorService
{
    public async ValueTask<bool> SignUpAsync(SignUpDetails signUpDetails, CancellationToken cancellationToken = default)
    {
        var foundUserId = await userService.GetIdByEmailAddressAsync(signUpDetails.EmailAddress, cancellationToken);

        if(foundUserId.HasValue)
            throw new InvalidOperationException("User with given email address already exists.");

        //Hash password
        var user = mapper.Map<User>(signUpDetails);
        var password = signUpDetails.AutoGeneratePassword
            ? passwordGeneratorService.GeneratePassword()
            : passwordGeneratorService.GetValidatePassword(signUpDetails.Password!, user);

        user.PasswordHash = passwordHasherService.HashPassword(password);

        //Create user
        return await accountAggregatorService.CreateUserAsync(user, cancellationToken);
    }
}
