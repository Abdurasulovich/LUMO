using AutoMapper;
using Lummo.Domain.Entities;
using Lummo.Infrastructure.StorageFiles.Settings;
using Microsoft.Extensions.Options;

namespace Lummo.Infrastructure.StorageFiles.Mappers;

public class StorageFileToUrlConverter(IOptions<StorageFileSettings> storageFileSettings, IOptions<ApiSettings> apiSettings)
    : IValueConverter<StorageFile, string>
{
    public string Convert(StorageFile sourceMember, ResolutionContext context)
    {
        var relativePath = Path.Combine(
            storageFileSettings.Value.LocationSettings.First(x => x.StorageFileType == sourceMember.FileType).FolderPath,
            sourceMember.FileName
        );

        var absoluteUrl = new Uri(new Uri(apiSettings.Value.BaseAddress), relativePath);
        return absoluteUrl.ToString();
    }
}
