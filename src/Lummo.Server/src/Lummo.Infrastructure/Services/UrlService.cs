using Lummo.Application.Common.Extensions;
using Lummo.Application.Common.Services.Interfaces;
using Lummo.Infrastructure.Common.Settings;
using Microsoft.Extensions.Options;

namespace Lummo.Infrastructure.Services;

public class UrlService(IOptions<UrlSettings> options) : IUrlService
{
    public ValueTask<string> GetUrlFromRelativePath(string relativePath)
    {
        return new(Path.Combine(options.Value.BaseUrl, relativePath.ToUrl()));
    }
}
