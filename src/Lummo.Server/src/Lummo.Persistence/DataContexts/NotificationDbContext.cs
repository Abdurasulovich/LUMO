using Lummo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lummo.Persistence.DataContexts;

public class NotificationDbContext(DbContextOptions<NotificationDbContext> options) : DbContext(options)
{

    public DbSet<EmailTemplate> EmailTemplates => Set<EmailTemplate>();

    public DbSet<EmailHistory> EmailHistories => Set<EmailHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationDbContext).Assembly);
    }
}
