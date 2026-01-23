using Lummo.Application.Common.Identity.Services.Interfaces;
using Lummo.Domain.Entities;
using Lummo.Domain.Enums;
using Lummo.Persistence.Caching.Brokers.Interfaces;
using Lummo.Persistence.DataContexts;
using Microsoft.EntityFrameworkCore;

namespace Lummo.Server.Data;

public static class SeedDataExtensions
{
    public static async ValueTask InitializeSeedAsync(this IServiceProvider serviceProvider)
    {
        var appDbContext = serviceProvider.GetRequiredService<AppDbContext>();

        var passwordHasherService = serviceProvider.GetRequiredService<IPasswordHasherService>();

        if (!await appDbContext.Roles.AnyAsync())
            await appDbContext.SeedRolesAsync();

        if (!await appDbContext.EmailTemplates.AnyAsync())
            await appDbContext.SeedEmailTemplatesAsync();

        // check change tracker and if changes exist, save changes to database
        if (appDbContext.ChangeTracker.HasChanges())
            appDbContext.SaveChanges();
    }
    /// <summary>
    /// Seeds the database with initial roles.
    /// </summary>
    /// <param name="dbContext"></param>
    private static async ValueTask SeedRolesAsync(this AppDbContext dbContext)
    {
        var roles = new List<Role>
        {
            new()
            {
                Id = Guid.Parse("7700e8af-6e37-4448-9409-8d9d03911732"),
                CreatedTime = DateTimeOffset.UtcNow,
                Type = RoleType.System
            },
            new()
            {
                Id = Guid.Parse("8346abd3-ec6e-4be4-9e17-784733a9e269"),
                CreatedTime = DateTimeOffset.UtcNow,
                Type = RoleType.Admin
            },
            new()
            {
                Id = Guid.Parse("22acb325-9a85-4ccd-afde-bbfcdd4ae53c"),
                CreatedTime = DateTimeOffset.UtcNow,
                Type = RoleType.Guest
            },
            new()
            {
                Id = Guid.Parse("a42302e1-ffa0-490b-8398-d4323bb3a9e4"),
                CreatedTime = DateTimeOffset.UtcNow,
                Type = RoleType.Host
            }
        };

        await dbContext.Roles.AddRangeAsync(roles);
        dbContext.SaveChanges();
    }

    /// <summary>
    /// Seeds the database with initial email templates.
    /// </summary>
    private static async ValueTask SeedEmailTemplatesAsync(this AppDbContext dbContext)
    {
        var emailTemplates = new List<EmailTemplate>
        {
            new()
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                CreatedTime = DateTimeOffset.UtcNow,
                TemplateType = NotificationTemplateType.EmailVerificationNotification,
                Subject = "Email Verification - {{UserName}}",
                Content = @"
                    <html>
                    <body style='font-family: Arial, sans-serif; padding: 20px;'>
                        <h2>Email Verification</h2>
                        <p>Hello {{UserName}},</p>
                        <p>Thank you for registering with Lummo!</p>
                        <p>Your verification code is: <strong>{{VerificationCode}}</strong></p>
                        <p>Or click the link below to verify your email:</p>
                        <p><a href='{{VerificationLink}}' style='background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Verify Email</a></p>
                        <p>This code will expire in 5 minutes.</p>
                        <br/>
                        <p>Best regards,<br/>Lummo Team</p>
                    </body>
                    </html>"
            },
            new()
            {
                Id = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
                CreatedTime = DateTimeOffset.UtcNow,
                TemplateType = NotificationTemplateType.SystemWelcomeNotification,
                Subject = "Welcome to Lummo - {{UserName}}",
                Content = @"
                    <html>
                    <body style='font-family: Arial, sans-serif; padding: 20px;'>
                        <h2>Welcome to Lummo!</h2>
                        <p>Hello {{UserName}},</p>
                        <p>Welcome to Lummo! We're excited to have you on board.</p>
                        <p>Start exploring our platform and discover amazing features.</p>
                        <br/>
                        <p>Best regards,<br/>Lummo Team</p>
                    </body>
                    </html>"
            }
        };

        await dbContext.EmailTemplates.AddRangeAsync(emailTemplates);
        dbContext.SaveChanges();
    }
}
