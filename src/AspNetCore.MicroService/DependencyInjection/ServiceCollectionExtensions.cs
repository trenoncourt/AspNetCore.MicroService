using System;
using AspNetCore.MicroService.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.MicroService.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMicroService(this IServiceCollection services, Action<MicroServiceOptionBuilder> handler = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
                
            var builder = new MicroServiceOptionBuilder(services);
            handler?.Invoke(builder);
            return services;
        }
    }
}