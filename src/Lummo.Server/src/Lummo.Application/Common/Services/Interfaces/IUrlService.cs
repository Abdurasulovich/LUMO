namespace Lummo.Application.Common.Services.Interfaces;

public interface IUrlService
{
    ValueTask<string> GetUrlFromRelativePath(string relativePath);
}
