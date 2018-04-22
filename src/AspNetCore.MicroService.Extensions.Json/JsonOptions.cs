using Newtonsoft.Json;

namespace AspNetCore.MicroService.Extensions.Json
{
    public class JsonOptions
    {
        public JsonSerializerSettings SerializerSettings { get; } = JsonSerializerSettingsProvider.CreateSerializerSettings();
    }
}