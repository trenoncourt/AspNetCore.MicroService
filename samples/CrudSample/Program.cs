using System;
using System.Collections.Generic;
using System.IO;
using AspNetCore.MicroService.DependencyInjection;
using AspNetCore.MicroService.Extensions.Crud;
using AspNetCore.MicroService.Extensions.Json.DependencyInjection;
using AspNetCore.MicroService.Routing.Builder;
using CrudSample.Dtos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CrudSample
{
    public class Program
    {
        private static IHostingEnvironment _hostingEnvironment;
        private static IConfiguration _configuration;
        public static void Main()
        {
            CreateWebHostBuilder()
                .Build()
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder()
        {
            return new WebHostBuilder()
                .UseKestrel(options => options.AddServerHeader = false)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    _configuration = config
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json",
                            optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables()
                        .Build();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    _hostingEnvironment = hostingContext.HostingEnvironment;
                    logging
                        .AddConfiguration(hostingContext.Configuration.GetSection("Logging"))
                        .AddConsole();
                })
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddRouting()
                        .AddMicroService(b => b.AddJson())
                        .AddCors()
                        .BuildServiceProvider();
                })
                .Configure(app =>
                {
                    app
                        .UseCors(b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin())
                        .Route("users", Users)
                            .Get()
                            .Post()
                        .SubRoute("{id}")
                            .Get()
                            .Put()
                            .Delete()
                        .Use();
                });
        }

        public static readonly List<User> Users = new List<User>
        {
            new User
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Age = 42,
                Email = "john-doe@foo.bar",
            },
            new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Doe",
                Age = 24,
                Email = "jane-doe@foo.bar",
            }
        };
    }
}