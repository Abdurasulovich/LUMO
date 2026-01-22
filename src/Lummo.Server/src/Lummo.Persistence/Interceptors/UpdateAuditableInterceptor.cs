using Lummo.Domain.Brokers;
using Lummo.Domain.Common.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Lummo.Persistence.Interceptors;

public class UpdateAuditableInterceptor(IRequestUserContextProvider userContextProvider)
    : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var auditableEntries =
            eventData.Context!.ChangeTracker.Entries<IAuditableEntity>().ToList();

        var creationAuditableEntries =
            eventData.Context!.ChangeTracker.Entries<ICreationAuditableEntity>().ToList();

        var modificationAuditableEntries =
            eventData.Context!.ChangeTracker.Entries<IModificationAuditableEntity>().ToList();

        auditableEntries.ForEach(entry =>
        {
            if (entry.State == EntityState.Modified)
                entry.Property(nameof(IAuditableEntity.ModifiedTime)).CurrentValue = DateTimeOffset.UtcNow;
            if (entry.State == EntityState.Added)
                entry.Property(nameof(IAuditableEntity.CreatedTime)).CurrentValue = DateTimeOffset.UtcNow;
        });

        creationAuditableEntries.ForEach(entry =>
        {
            if (entry.State == EntityState.Added)
                entry.Property(nameof(ICreationAuditableEntity.CreatedByUserid)).CurrentValue =
                userContextProvider.GetUserId();
        });

        modificationAuditableEntries.ForEach(entry =>
        {
            if (entry.State == EntityState.Modified)
                entry.Property(nameof(IModificationAuditableEntity.ModifiedByUserId)).CurrentValue =
                userContextProvider.GetUserId();
        });

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
