using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AspNetCore.MicroService.Extensions.Json
{
    public static class JsonSerializerSettingsProvider
    {
        private const int DefaultMaxDepth = 32;
        
        public static JsonSerializerSettings CreateSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),

                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,

                // Stackoverflow exceptions prevention
                // from deserialization errors that might occur from deeply nested objects.
                MaxDepth = DefaultMaxDepth,
            };
        }
    }
}