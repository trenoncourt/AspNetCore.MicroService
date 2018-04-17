using System.IO;
using AspNetCore.MicroService.Routing.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
                        .AddCors()
                        .BuildServiceProvider();
                })
                .Configure(app =>
                {
                    
                    app
                        .UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin())
                        .Route("funny")
                            .Get(async c => await c.Response.WriteAsync("yeah!"))
                            .Use();
                })
                .Build()
                .Run();
        }
    }
}