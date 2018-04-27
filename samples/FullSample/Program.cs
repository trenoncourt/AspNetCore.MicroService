using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AspNetCore.MicroService.DependencyInjection;
using AspNetCore.MicroService.Extensions.Json;
using AspNetCore.MicroService.Extensions.Json.DependencyInjection;
using AspNetCore.MicroService.Routing.Builder;
using FullSample.Dtos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FullSample
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
                        .Route("users")
                            .Get(async c => await c.WriteJsonAsync(Users))
                            .Post(c =>
                            {
                                var user = c.ReadJsonBody<User>();
                                if (user != null)
                                {
                                    CreateUser(user);
                                    c.Response.Headers["Location"] = $"/users/{user.Id}";
                                    c.Response.StatusCode = 201;
                                }
                                else c.Response.StatusCode = 400;
                            })
                        .Route("users/{id}")
                            .Get(async c =>
                            {
                                User user = Users.FirstOrDefault(u => u.Id.ToString() == c.GetRouteValue("id").ToString());
                                if (user == null) c.Response.StatusCode = 404;
                                else
                                    await c.WriteJsonAsync(user);
                            })
                            .Put(c =>
                            {
                                string userId = c.GetRouteValue("id").ToString();
                                User existingUser = Users.FirstOrDefault(u => u.Id.ToString() == userId);
                                if (existingUser == null) c.Response.StatusCode = 404;
                                else
                                {
                                    var user = c.ReadJsonBody<User>();
                                    if (user.Id.ToString() != userId) c.Response.StatusCode = 400;
                                    else
                                    {
                                        UpdateUser(existingUser, user);
                                        c.Response.StatusCode = 204;
                                    }
                                }
                            })
                            .Delete(c =>
                            {
                                User user = Users.FirstOrDefault(u => u.Id.ToString() == c.GetRouteValue("id").ToString());
                                if (user == null) c.Response.StatusCode = 404;
                                else
                                {
                                    Users.Remove(user);
                                    c.Response.StatusCode = 204;
                                }
                            })
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

        private static void CreateUser(User user)
        {
            user.Id = Guid.NewGuid();
            Users.Add(user);
        }

        private static void UpdateUser(User existingUser, User newUser)
        {
            existingUser.FirstName = newUser.FirstName;
            existingUser.LastName = newUser.LastName;
            existingUser.Email = newUser.Email;
            existingUser.Age = newUser.Age;
        }
    }
}