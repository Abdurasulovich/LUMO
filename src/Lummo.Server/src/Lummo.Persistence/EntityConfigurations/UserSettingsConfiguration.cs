using Lummo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lummo.Persistence.EntityConfigurations;

public class UserSettingsConfiguration : IEntityTypeConfiguration<UserSettings>
{
    public void Configure(EntityTypeBuilder<UserSettings> builder)
    {
        builder.HasOne(userSettings=>userSettings.User)
            .WithOne(user => user.UserSettings)
            .HasForeignKey<UserSettings>(us => us.UserId);
    }
}
