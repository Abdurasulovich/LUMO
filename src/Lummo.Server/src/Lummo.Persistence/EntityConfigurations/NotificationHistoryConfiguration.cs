using Lummo.Domain.Entities;
using Lummo.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lummo.Persistence.EntityConfigurations;

public class NotificationHistoryConfiguration : IEntityTypeConfiguration<NotificationHistory>
{
    public void Configure(EntityTypeBuilder<NotificationHistory> builder)
    {
        builder
            .HasDiscriminator(history => history.Type)
            .HasValue<EmailHistory>(NotificationType.Email);
    }
}
