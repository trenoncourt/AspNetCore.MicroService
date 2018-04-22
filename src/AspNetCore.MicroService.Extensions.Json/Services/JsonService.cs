using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace AspNetCore.MicroService.Extensions.Json.Services
{
    internal class JsonService : IJsonService
    {
        private readonly JsonSerializer _jsonSerializer;

        public JsonService(JsonOptions jsonOptions)
        {
            _jsonSerializer = JsonSerializer.Create(jsonOptions?.SerializerSettings);
        }
         
        public Task WriteJsonToStreamAsync(Stream stream, object value, JsonSerializerSettings jsonSerializerSettings = null)
        {
            var jsonSerializer = jsonSerializerSettings != null
                ? JsonSerializer.Create(jsonSerializerSettings)
                : _jsonSerializer;
             
            using (var writer = new HttpResponseStreamWriter(stream, Encoding.UTF8)) 
            { 
                using (var jsonWriter = new JsonTextWriter(writer) { CloseOutput = false, AutoCompleteOnClose = false }) 
                { 
                    jsonSerializer.Serialize(jsonWriter, value); 
                } 
                return writer.FlushAsync(); 
            } 
        } 
  
        public T ReadJsonFromStream<T>(Stream stream, JsonSerializerSettings jsonSerializerSettings = null) 
        { 
            var jsonSerializer = jsonSerializerSettings != null
                ? JsonSerializer.Create(jsonSerializerSettings)
                : _jsonSerializer;
            
            using (var streamReader = new StreamReader(stream)) 
            { 
                using (var jsonTextReader = new JsonTextReader(streamReader) { CloseInput = false }) 
                { 
                    var model = jsonSerializer.Deserialize<T>(jsonTextReader);

                    return model;
                } 
            } 
        } 
    }
}