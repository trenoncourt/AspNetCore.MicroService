using System;
using AspNetCore.MicroService.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.MicroService.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMicroService(this IServiceCollection services, Action<MicroServiceBuilder> handler = null, Action<MicroServiceSettings> settingsHandler = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            
            var settings = new MicroServiceSettings();
            settingsHandler?.Invoke(settings);
            services.AddSingleton(settings);
                
            var builder = new MicroServiceBuilder(services);
            handler?.Invoke(builder);
            return services;
        }
    }
}