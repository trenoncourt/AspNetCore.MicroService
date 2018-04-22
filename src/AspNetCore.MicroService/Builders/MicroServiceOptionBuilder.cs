using System;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.MicroService.Builders
{
    public class MicroServiceOptionBuilder
    {
        public MicroServiceOptionBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
        
        public IServiceCollection Services { get; }
    }
}