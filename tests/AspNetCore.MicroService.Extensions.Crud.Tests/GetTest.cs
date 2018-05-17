using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AspNetCore.MicroService.DependencyInjection;
using AspNetCore.MicroService.Extensions.Json;
using AspNetCore.MicroService.Extensions.Json.DependencyInjection;
using AspNetCore.MicroService.Routing.Builder;
using CrudSample;
using CrudSample.Dtos;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace AspNetCore.MicroService.Extensions.Crud.Tests
{
    public class GetTest
    {
        [Fact]
        public async Task StartServerWithGetMethod_GetUsers_VerifyResultContainsUsers()
        {
            // Arrange
            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(s => s.AddRouting().AddMicroService(b => b.AddJson()).BuildServiceProvider())
                .Configure(app => app.Route("users").Get(Program.Users).Use()));

            HttpClient client = server.CreateClient();
            
            // Act
            var response = await client.GetAsync("/users");
            response.EnsureSuccessStatusCode();

            string responseData = await response.Content.ReadAsStringAsync();
            
            // Assert
            response.StatusCode.Should().Be(200);
            response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
            responseData.Should().Be(JsonConvert.SerializeObject(Program.Users, JsonSerializerSettingsProvider.CreateSerializerSettings()));
        }
        
        [Fact]
        public async Task StartServerWithGetByIdMethod_GetUserRoute_VerifyResultContainsTheUser()
        {
            // Arrange
            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(s => s.AddRouting().AddMicroService(b => b.AddJson()).BuildServiceProvider())
                .Configure(app => app.Route("users/{id}").Get(Program.Users).Use()));

            HttpClient client = server.CreateClient();
            
            // Act
            User user = CrudSample.Program.Users.First();
            var response = await client.GetAsync($"/users/{user.Id}");
            response.EnsureSuccessStatusCode();

            string responseData = await response.Content.ReadAsStringAsync();
            
            // Assert
            response.StatusCode.Should().Be(200);
            response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
            responseData.Should().Be(JsonConvert.SerializeObject(user, JsonSerializerSettingsProvider.CreateSerializerSettings()));
        }
        
        [Fact]
        public async Task StartServerWithGetByIdMethod_GetUserWithWrongId_VerifyResponseIsNotFound()
        {
            // Arrange
            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(s => s.AddRouting().AddMicroService(b => b.AddJson()).BuildServiceProvider())
                .Configure(app => app.Route("users/{id}").Get(Program.Users).Use()));

            HttpClient client = server.CreateClient();
            
            // Act
            var response = await client.GetAsync($"/users/0");

            string responseData = await response.Content.ReadAsStringAsync();
            
            // Assert
            response.StatusCode.Should().Be(404);
            responseData.Should().BeEmpty();
        }
    }
}