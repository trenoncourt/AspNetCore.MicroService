using System.Collections.Generic;
using System.IO;
using System.Text;
using AspNetCore.MicroService.Routing.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
                        .Route("users")
                            .Get(async c =>
                            {
                                await c.Response.WriteAsync("lot of yeah!");
                            })
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
                            .BeforeEach(async c =>
                            {
                                if (c.Request.Method != "GET")
                                {
                                    using (var streamReader = new StreamReader(c.Request.Body, Encoding.UTF8))
                                    {
                                        string body = await streamReader.ReadToEndAsync();
                                        Events.Add(new Event {Method = c.Request.Method, Body = body.Replace("\n", "").Replace("\t", ""), Path = c.Request.Path.ToString()}); 
                                    }
                                }
                                c.Response.Headers.Add("before-each", "ok");
                            })
                        .Route("events")
                            .Get(async c =>
                            {
                                c.Response.ContentType = "application/json";
                                await c.Response.WriteAsync(JsonConvert.SerializeObject(Events));
                            })
                        .Use();
                })
                .Build()
                .Run();
        }

        public static readonly List<Event> Events = new List<Event>();
        
        
    }
}