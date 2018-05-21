using AspNetCore.MicroService.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.MicroService.Routing.Abstractions
{
    public static class MicroServiceBuilderExtensions
    {
        public static MicroServiceBuilder AddMetadatas(this MicroServiceBuilder builder)
        {
            builder.Settings.EnableMetadatas = true;
            
            builder.Services.AddSingleton(new MicroServiceMetadatas());

            return builder;
        }
    }
}