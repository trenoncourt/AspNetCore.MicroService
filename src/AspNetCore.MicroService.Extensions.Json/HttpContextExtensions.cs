using System.Threading.Tasks;
using AspNetCore.MicroService.Extensions.Json.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AspNetCore.MicroService.Extensions.Json
{
    public static class HttpContextExtensions
    {
        private static readonly JsonSerializer JsonSerializer = JsonSerializer.Create(new JsonSerializerSettings { 
            NullValueHandling = NullValueHandling.Ignore, 
            DefaultValueHandling = DefaultValueHandling.Ignore, 
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore, 
            ContractResolver = new CamelCasePropertyNamesContractResolver() 
        }); 
         
        public static Task WriteJsonAsync(this HttpContext httpContext, object value, JsonSerializerSettings jsonSerializerSettings = null)
        {
            var jsonService = httpContext.RequestServices.GetService<IJsonService>();
             
            httpContext.Response.ContentType = "application/json";
            return jsonService.WriteJsonToStreamAsync(httpContext.Response.Body, value, jsonSerializerSettings);
        } 
  
        public static T ReadJsonBody<T>(this HttpContext httpContext, JsonSerializerSettings jsonSerializerSettings = null) 
        {
            var jsonService = httpContext.RequestServices.GetService<IJsonService>();
            return jsonService.ReadJsonFromStream<T>(httpContext.Request.Body);
        } 
    }
}