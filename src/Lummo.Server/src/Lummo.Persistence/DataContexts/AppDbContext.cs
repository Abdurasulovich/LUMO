using Lummo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lummo.Persistence.DataContexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    #region Identity
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<UserSettings> UserSettings => Set<UserSettings>();
    #endregion

    #region Notifications
    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
    public DbSet<EmailTemplate> EmailTemplates => Set<EmailTemplate>();
    public DbSet<NotificationHistory> NotificationHistories => Set<NotificationHistory>();
    public DbSet<EmailHistory> EmailHistories => Set<EmailHistory>();
    #endregion

    #region Verifications
    public DbSet<VerificationCode> VerificationCodes => Set<VerificationCode>();
    public DbSet<UserInfoVerificationCode> UserInfoVerificationCodes => Set<UserInfoVerificationCode>();
    #endregion

    #region Media
    public DbSet<StorageFile> StorageFiles => Set<StorageFile>();
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
