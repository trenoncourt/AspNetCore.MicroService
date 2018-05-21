using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AspNetCore.MicroService.DependencyInjection;
using AspNetCore.MicroService.Extensions.Json;
using AspNetCore.MicroService.Extensions.Json.DependencyInjection;
using AspNetCore.MicroService.Routing.Builder;
using AspNetCore.MicroService.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SwaggerSample.Dtos;
using Swashbuckle.AspNetCore.Swagger;

namespace SwaggerSample
{
    public class Program
    {
        private static IHostingEnvironment _hostingEnvironment;
        private static IConfiguration _configuration;
        public static void Main()
        {
            new WebHostBuilder()
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
                        .AddMicroService(x => x.AddJson().AddMetadatas().AddSwagger())
                        .AddCors()
                        .BuildServiceProvider();
                })
                .Configure(app =>
                {
                    
                    app
                        .UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin())
                        .Route("users", Users)
                        .Get(async x => await x.WriteJsonAsync(Users))
                        .Post(async c =>
                        {
                            c.Request.EnableRewind();
                            await c.Request.Body.CopyToAsync(c.Response.Body);
                        })
                        .SubRoute("{id}")
                        .Get(async c => await c.Response.WriteAsync("just one yeah!"))
                        .Put(async c =>
                        {
                            c.Request.EnableRewind();
                            await c.Request.Body.CopyToAsync(c.Response.Body);
                        })
                        .Delete(c => c.Response.StatusCode = 204)
                        .UseSwagger()
                        .Use()
                        .UseSwagger()
                        .UseSwaggerUI(c =>
                        {
                            c.RoutePrefix = "swagger"; // serve the UI at root
                            c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");

                            c.ShowExtensions();
                        });;
                })
                .Build()
                .Run();
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