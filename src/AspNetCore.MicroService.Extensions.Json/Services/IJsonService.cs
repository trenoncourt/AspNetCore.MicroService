using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AspNetCore.MicroService.Extensions.Json.Services
{
    public interface IJsonService
    {
        Task WriteJsonToStreamAsync(Stream stream, object value, JsonSerializerSettings jsonSerializerSettings = null);

        T ReadJsonFromStream<T>(Stream stream, JsonSerializerSettings jsonSerializerSettings = null);
    }
}