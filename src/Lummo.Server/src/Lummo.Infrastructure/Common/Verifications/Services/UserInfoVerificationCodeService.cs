using FluentValidation;
using Lummo.Application.Common.Verifications.Services.Interfaces;
using Lummo.Domain.Entities;
using Lummo.Domain.Enums;
using Lummo.Infrastructure.Common.Settings;
using Lummo.Persistence.Extensions;
using Lummo.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Polly;
using System.Security.Cryptography;

namespace Lummo.Infrastructure.Common.Verifications.Services;

public class UserInfoVerificationCodeService(
    IOptions<VerificationCodeSettings> verificationSettings,
    IValidator<UserInfoVerificationCode> userInfoVerificationCodeValidator,
    IUserInfoVerificationCodeRepository userInfoVerificationCodeRepository
    ) : IUserInfoVerificationCodeService
{
    private readonly VerificationCodeSettings _verificationSettings = verificationSettings.Value;
    public async ValueTask<UserInfoVerificationCode> CreateAsync(VerificationCodeType codeType, Guid userId, CancellationToken cancellationToken = default)
    {
        var verificationCodeValue = default(string);
        //TODO: code generate process with polly
        var retryPolicy = Policy
            .HandleResult<string>(code => string.IsNullOrEmpty(code))
            .RetryAsync(5, (result, retryCount) =>
            {
                Console.WriteLine($"Retry {retryCount}: verification code is empty");
            });

        await retryPolicy.ExecuteAsync(async () =>
        {
            var verificationCodes = Generate();
            var existingCodes = await userInfoVerificationCodeRepository
            .Get(code => verificationCodes.Contains(code.Code)).ToListAsync(cancellationToken);

            verificationCodeValue = verificationCodes.Except(existingCodes.Select(code => code.Code)).FirstOrDefault();

            return verificationCodeValue!;
        });

        var verificationCode = new UserInfoVerificationCode
        {
            Code = verificationCodeValue,
            CodeType = codeType,
            UserId = userId,
            IsActive = true,
            VerificationLink = $"{_verificationSettings.VerificationLink}/{verificationCodeValue}",
            ExpiryTime = DateTime.UtcNow.AddSeconds(_verificationSettings.VerificationCodeExpiryTimeInSeconds)
        };

        var validationResult = userInfoVerificationCodeValidator.Validate(verificationCode);
        if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

        await userInfoVerificationCodeRepository.CreateAsync(verificationCode, cancellationToken: cancellationToken);

        return verificationCode;
    }

    public ValueTask DeactivateAsync(Guid codeId, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        return userInfoVerificationCodeRepository.DeactivateAsync(codeId, saveChanges, cancellationToken);
    }

    public IList<string> Generate()
    {
        var codes = new List<string>();
        for (var index = 0; index < 10; index++)
            codes.Add(string.Join("", RandomNumberGenerator.GetBytes(_verificationSettings.VerificationCodeLength)
                .Select(@char => @char % 10)));

        return codes;
    }

    public async ValueTask<(UserInfoVerificationCode Code, bool IsValid)> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var verificationCode = await userInfoVerificationCodeRepository.Get(verificationCode => verificationCode.Code == code, true)
            .FirstOrDefaultAsync(cancellationToken) ?? throw new InvalidOperationException();

        return (verificationCode, verificationCode.IsActive && verificationCode.ExpiryTime > DateTimeOffset.UtcNow);
    }

    public async ValueTask<VerificationType?> GetVerificationTypeAsync(string code, CancellationToken cancellationToken = default)
    {
        var verificationCode = await userInfoVerificationCodeRepository
            .Get(verification => verification.Code == code, true)
            .Select(
            verificationCode => new
            {
                verificationCode.Id,
                verificationCode.Type
            })
            .FirstOrDefaultAsync(cancellationToken);

        return verificationCode?.Type;
    }
}
