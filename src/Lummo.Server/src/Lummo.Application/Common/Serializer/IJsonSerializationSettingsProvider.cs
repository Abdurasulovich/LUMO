using Newtonsoft.Json;

namespace Lummo.Application.Common.Serializer;

public interface IJsonSerializationSettingsProvider
{
    JsonSerializerSettings Get(bool clone = false);
}
