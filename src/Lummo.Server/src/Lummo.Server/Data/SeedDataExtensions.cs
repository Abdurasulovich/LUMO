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
}
