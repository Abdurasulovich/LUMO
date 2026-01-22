namespace Lummo.Infrastructure.StorageFiles.Settings;

public class StorageFileSettings
{
    public IEnumerable<StorageFileLocationSettings> LocationSettings { get; set; } = default!;
}
