using Lummo.Domain.Common.Entities;
using Lummo.Domain.Enums;

namespace Lummo.Domain.Entities;

public class StorageFile :  Entity
{
    public string FileName { get; set; } = default!;
    public StorageFileType FileType { get; set; }

}
