using Lummo.Domain.Enums;

namespace Lummo.Infrastructure.StorageFiles.Settings;

public class StorageFileLocationSettings
{
    public StorageFileType StorageFileType { get; set; }
    public string FolderPath { get; set; } = default!;
}
