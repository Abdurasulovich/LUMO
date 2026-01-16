using Force.DeepCloner;
using Lummo.Application.Common.Serializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lummo.Infrastructure.Serializers;

public class JsonSerializationSettingsProvider : IJsonSerializationSettingsProvider
{
    private readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        Formatting = Formatting.Indented,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore
    };
    public JsonSerializerSettings Get(bool clone = false)
    {
        return clone ? _jsonSerializerSettings.DeepClone() : _jsonSerializerSettings;
    }
}
