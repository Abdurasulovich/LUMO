using Lummo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lummo.Persistence.DataContexts;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<UserSettings> UsersSettings => Set<UserSettings>();

    public DbSet<UserInfoVerificationCode> UserInfoVerificationCodes => Set<UserInfoVerificationCode>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
    }
}
