﻿using System;
using AspNetCore.MicroService.Builders;
using AspNetCore.MicroService.Routing.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AspNetCore.MicroService.Swagger
{
    public static class MicroServiceBuilderExtensions
    {
        public static MicroServiceBuilder AddMetadatas(this MicroServiceBuilder builder)
        {
            builder.Settings.EnableMetadatas = true;
            
            builder.Services.AddSingleton(new MicroServiceMetadatas());

            return builder;
        }

        public static MicroServiceBuilder AddSwagger(this MicroServiceBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            builder.Services.Configure(new Action<SwaggerGenOptions>(c =>
            {
                c.SwaggerDoc("v1",
                    new Info
                    {
                        Version = "v1",
                        Title = "Swashbuckle Sample API",
                        Description = "A sample API for testing Swashbuckle",
                        TermsOfService = "Some terms ..."
                    }
                );
            }));
            builder.Services.AddTransient<ISwaggerProvider>(sp => new MicroServiceSwaggerGenerator(sp));
                
            return builder;
        }
    }
}