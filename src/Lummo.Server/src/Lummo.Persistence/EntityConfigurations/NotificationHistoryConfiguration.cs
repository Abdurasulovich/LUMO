using Lummo.Domain.Entities;
using Lummo.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lummo.Persistence.EntityConfigurations;

///<summary>
/// Entity type configuration for the NotificationHistory class.
///</summary>
public class NotificationHistoryConfiguration : IEntityTypeConfiguration<NotificationHistory>
{
    ///<summary>
    /// Configures the entity type properties and behavior for the NotificationHistory class.
    ///</summary>
    ///<param name="builder">The entity type builder used to configure the NotificationHistory entity.</param>
    public void Configure(EntityTypeBuilder<NotificationHistory> builder)
    {
        builder.Property(template => template.Content).HasMaxLength(129_536);

        builder.ToTable("NotificationHistories").HasDiscriminator(history => history.Type).HasValue<EmailHistory>(NotificationType.Email);

        builder.HasOne<NotificationTemplate>(history => history.Template)
            .WithMany(template => template.Histories)
            .HasForeignKey(history => history.TemplateId);

        builder.HasOne<User>().WithMany().HasForeignKey(history => history.SenderUserId);
        builder.HasOne<User>().WithMany().HasForeignKey(history => history.ReceiverUserId);
    }
}
