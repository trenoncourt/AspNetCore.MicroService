using System;
using AspNetCore.MicroService.Builders;
using AspNetCore.MicroService.Extensions.Json.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace AspNetCore.MicroService.Extensions.Json.DependencyInjection
{
    public static class MicroServiceBuilderExtensions
    {
        public static MicroServiceBuilder AddJson(this MicroServiceBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            
            builder.Services.AddSingleton(new JsonOptions());
            builder.Services.AddSingleton<IJsonService, JsonService>();
                
            return builder;
        }
        
        public static MicroServiceBuilder AddJson(this MicroServiceBuilder builder, Action<JsonSerializerSettings> handler)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var jsonOptions = new JsonOptions();
            handler(jsonOptions.SerializerSettings);
            builder.Services.AddSingleton(jsonOptions);
            builder.Services.AddSingleton<IJsonService, JsonService>();
                
            return builder;
        }
    }
}