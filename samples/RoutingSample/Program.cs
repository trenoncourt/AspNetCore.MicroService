using System.IO;
using AspNetCore.MicroService.DependencyInjection;
using AspNetCore.MicroService.Routing.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace RoutingSample
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
                        .AddMicroService()
                        .AddCors()
                        .BuildServiceProvider();
                })
                .Configure(app =>
                {
                    
                    app
                        .UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin())
                        .Route("users")
                            .Get(async c => await c.Response.WriteAsync("lot of yeah!"))
                            .Post(async c =>
                            {
                                c.Request.EnableRewind();
                                await c.Request.Body.CopyToAsync(c.Response.Body);
                            })
                        .Route("users/{id}")
                            .Get(async c => await c.Response.WriteAsync("just one yeah!"))
                            .Put(async c =>
                            {
                                c.Request.EnableRewind();
                                await c.Request.Body.CopyToAsync(c.Response.Body);
                            })
                            .Delete(c => c.Response.StatusCode = 204)
                            .Use();
                })
                .Build()
                .Run();
        }
    }
}