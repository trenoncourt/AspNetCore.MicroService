using System;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.MicroService.Builders
{
    public class MicroServiceBuilder
    {
        public MicroServiceBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
        
        public IServiceCollection Services { get; }
    }
}