using Azure.Core;
using Lummo.Application.Common.Identity.Models;
using Lummo.Application.Common.Identity.Services.Interfaces;
using Lummo.Application.Common.Notifications.Models;
using Lummo.Application.Common.Notifications.Services.Interfaces;
using Lummo.Application.Common.Verifications.Services.Interfaces;
using Lummo.Domain.Entities;
using Lummo.Domain.Enums;
using Lummo.Infrastructure.Common.Settings;
using Lummo.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.CodeDom;

namespace Lummo.Infrastructure.Common.Identity.Services;

public class AccountService(
    IUserService userService,
    IUserRepository userRepository,
    IUserSettingsService userSettingsService,
    IUserInfoVerificationCodeService userInfoVerificationCodeService,
    IEmailSenderService emailSenderService,
    IEmailTemplateService emailTemplateService,
    IEmailRenderingService emailRenderingService,
    IPasswordHasherService passwordHasherService,
    IOptions<SmtpEmailSenderSettings> smtpEmailSenderSettings
    ) : IAccountService
{
    private readonly SmtpEmailSenderSettings _smtpEmailSenderSettings = smtpEmailSenderSettings.Value;

    public async ValueTask<User> CreateUserAsync(User user, bool skipEmailVerification = false, CancellationToken cancellationToken = default)
    {
        // Agar OAuth provider (Google, Apple, etc.) orqali bo'lsa, email allaqachon verified
        if (skipEmailVerification)
            user.IsEmailAddressVerified = true;

        var createdUser = await userService.CreateAsync(user, cancellationToken: cancellationToken);

        await userSettingsService.CreateAsync(
            new UserSettings
            {
                UserId = createdUser.Id
            },
            cancellationToken: cancellationToken
        );

        // OAuth provider orqali ro'yxatdan o'tgan userlar uchun verification skip
        if (skipEmailVerification)
            return createdUser;

        // Generate verification code
        var verificationCode = await userInfoVerificationCodeService.CreateAsync(
            VerificationCodeType.EmailAddressVerification,
            createdUser.Id,
            cancellationToken: cancellationToken
        );

        // Get email template
        var emailTemplate = await emailTemplateService.GetByTypeAsync(
            NotificationTemplateType.EmailVerificationNotification,
            true,
            cancellationToken
        );

        if (emailTemplate is not null)
        {
            // Create email message with template and variables
            var emailMessage = new EmailMessage
            {
                SenderEmailAddress = _smtpEmailSenderSettings.CredentialAddress,
                ReceiverEmailAddress = createdUser.EmailAddress,
                Template = emailTemplate,
                Variables = new Dictionary<string, string>
                {
                    { "UserName", $"{createdUser.FirstName} {createdUser.LastName}" },
                    { "VerificationCode", verificationCode.Code },
                    { "VerificationLink", verificationCode.VerificationLink }
                }
            };

            // Render email with verification code
            await emailRenderingService.RenderAsync(emailMessage, cancellationToken);

            // Send verification email
            await emailSenderService.SendAsync(emailMessage, cancellationToken);
        }

        return createdUser;
    }

    public async ValueTask<User?> GetUserByEmailAddressAsync(string emailAddress, bool asNoTracking = false, CancellationToken cancellationToken = default)
        => await userRepository.Get(asNoTracking: asNoTracking)
            .FirstOrDefaultAsync(user => user.EmailAddress == emailAddress, cancellationToken: cancellationToken);

    public async ValueTask<bool> VerifyUserAsync(EmailVerificationDetails verificationDetails, CancellationToken cancellationToken = default)
    {
        var userVerifyCode = await userInfoVerificationCodeService.GetByCodeAsync(verificationDetails.VerificationCode, cancellationToken: cancellationToken);

        if (!userVerifyCode.IsValid)
            return false;

        var user = await userService.GetByIdAsync(userVerifyCode.Code.UserId, cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException();

        // Email manzilini tekshirish - kod faqat shu emailga tegishli bo'lishi kerak
        if (!string.Equals(user.EmailAddress, verificationDetails.EmailAddress, StringComparison.OrdinalIgnoreCase))
            return false;

        switch (userVerifyCode.Code.CodeType)
        {
            case VerificationCodeType.EmailAddressVerification:
                user.IsEmailAddressVerified = true;
                await userService.UpdateAsync(user, false, cancellationToken: cancellationToken);
                break;
            default:
                throw new NotSupportedException();
        }

        await userInfoVerificationCodeService.DeactivateAsync(userVerifyCode.Code.Id, cancellationToken: cancellationToken);
        return true;
    }

    public async ValueTask<bool> ResendVerificationCodeAsync(ResendVerificationCodeRequest request, CancellationToken cancellationToken = default)
    {
        // Find user by email
        var user = await GetUserByEmailAddressAsync(request.EmailAddress, true, cancellationToken);

        if (user is null)
            return false;

        // Check if already verified
        if (user.IsEmailAddressVerified)
            return false;

        // Generate new verification code
        var verificationCode = await userInfoVerificationCodeService.CreateAsync(
            VerificationCodeType.EmailAddressVerification,
            user.Id,
            cancellationToken: cancellationToken
        );

        // Get email template
        var emailTemplate = await emailTemplateService.GetByTypeAsync(
            NotificationTemplateType.EmailVerificationNotification,
            true,
            cancellationToken
        );

        if (emailTemplate is null)
            return false;

        // Create email message with template and variables
        var emailMessage = new EmailMessage
        {
            SenderEmailAddress = _smtpEmailSenderSettings.CredentialAddress,
            ReceiverEmailAddress = user.EmailAddress,
            Template = emailTemplate,
            Variables = new Dictionary<string, string>
            {
                { "UserName", $"{user.FirstName} {user.LastName}" },
                { "VerificationCode", verificationCode.Code },
                { "VerificationLink", verificationCode.VerificationLink }
            }
        };

        // Render email with verification code
        await emailRenderingService.RenderAsync(emailMessage, cancellationToken);

        // Send verification email
        return await emailSenderService.SendAsync(emailMessage, cancellationToken);
    }

    public async ValueTask<bool> ForgotPasswordVerifyEmailAsync(
        ForgotPasswordEmailVerificationDetails verificationEmail,
        CancellationToken cancellationToken = default)
    {
        var user = await GetUserByEmailAddressAsync(
            verificationEmail.EmailAddress, true, cancellationToken);

        if (user is null)
            return false;

        var verificationCode = await userInfoVerificationCodeService.CreateAsync(
            VerificationCodeType.PasswordReset,
            user.Id,
            cancellationToken: cancellationToken
        );

        var emailTemplate = await emailTemplateService.GetByTypeAsync(
            NotificationTemplateType.PasswordResetNotification,
            true,
            cancellationToken
        );

        if (emailTemplate is null)
            return false;

        var emailMessage = new EmailMessage
        {
            SenderEmailAddress = _smtpEmailSenderSettings.CredentialAddress,
            ReceiverEmailAddress = user.EmailAddress,
            Template = emailTemplate,
            Variables = new Dictionary<string, string>
        {
            { "UserName", $"{user.FirstName} {user.LastName}" },
            { "VerificationCode", verificationCode.Code },
            { "VerificationLink", verificationCode.VerificationLink }
        }
        };

        await emailRenderingService.RenderAsync(emailMessage, cancellationToken);

        return await emailSenderService.SendAsync(emailMessage, cancellationToken);
    }

    public async ValueTask<bool> ResetPasswordAsync(ResetPasswordDetails resetPasswordDetails, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userService.GetByIdAsync(resetPasswordDetails.UserId, cancellationToken: cancellationToken);
            if (user is not null)
            {
                user.UserCredentials = new UserCredentials
                {
                    PasswordHash = passwordHasherService.HashPassword(resetPasswordDetails.NewPassword)
                };

                await userService.UpdateAsync(user, false, cancellationToken: cancellationToken);
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            throw new NotImplementedException("Password reset failed. User not found or update failed.", ex);
        }
    }

    public async ValueTask<bool> ForgotPasswordConfirmEmailAsync(EmailVerificationDetails verificationDetails, CancellationToken cancellationToken = default)
    {
        var userVerifyCode = await userInfoVerificationCodeService.GetByCodeAsync(verificationDetails.VerificationCode, cancellationToken: cancellationToken);

        if (!userVerifyCode.IsValid)
            return false;

        var user = await userService.GetByIdAsync(userVerifyCode.Code.UserId, cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException();

        // Email manzilini tekshirish - kod faqat shu emailga tegishli bo'lishi kerak
        if (!string.Equals(user.EmailAddress, verificationDetails.EmailAddress, StringComparison.OrdinalIgnoreCase))
            return false;

        switch (userVerifyCode.Code.CodeType)
        {
            case VerificationCodeType.PasswordReset:
                user.IsEmailAddressVerified = true;
                await userService.UpdateAsync(user, false, cancellationToken: cancellationToken);
                break;
            default:
                throw new NotSupportedException();
        }

        await userInfoVerificationCodeService.DeactivateAsync(userVerifyCode.Code.Id, cancellationToken: cancellationToken);
        return true;
    }
}
