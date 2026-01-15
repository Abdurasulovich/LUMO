using Lummo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Lummo.Persistence.EntityConfigurations;

public class EmailHistoryConfiguration : IEntityTypeConfiguration<EmailHistory>
{
    ///<summary>
    /// Configures the entity type properties and behavior for the EmailHistory class.
    ///</summary>
    ///<param name="builder">The entity type builder used to configure the EmailHistory entity.</param>
    public void Configure(EntityTypeBuilder<EmailHistory> builder)
    {
        builder.Property(template => template.SenderEmailAddress).IsRequired().HasMaxLength(256);
        builder.Property(template => template.ReceiverEmailAddress).IsRequired().HasMaxLength(256);
        builder.Property(template => template.Subject).IsRequired().HasMaxLength(256);
    }
}