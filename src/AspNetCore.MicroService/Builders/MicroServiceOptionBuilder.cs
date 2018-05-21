using System;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.MicroService.Builders
{
    public class MicroServiceBuilder
    {
        public MicroServiceBuilder(IServiceCollection services, MicroServiceSettings settings)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            Settings = settings;
        }
        
        public IServiceCollection Services { get; }

        public MicroServiceSettings Settings { get; }
    }
}