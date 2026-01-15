using Lummo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lummo.Persistence.EntityConfigurations;
///<summary>
/// Entity type configuration for the EmailTemplate class.
///</summary>
public class EmailTemplateConfiguration : IEntityTypeConfiguration<EmailTemplate>
{
    ///<summary>
    /// Configures the entity type properties and behavior for the EmailTemplate class.
    ///</summary>
    ///<param name="builder">The entity type builder used to configure the EmailTemplate entity.</param>
    public void Configure(EntityTypeBuilder<EmailTemplate> builder)
    {
        builder.Property(template => template.Subject).IsRequired().HasMaxLength(256);
    }
}