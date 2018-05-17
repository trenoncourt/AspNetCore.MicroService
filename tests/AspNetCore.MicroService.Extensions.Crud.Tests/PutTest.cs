using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.MicroService.DependencyInjection;
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
    public class PutTest
    {
        [Fact]
        public async Task StartServerWithPutMethod_PutUserWithWrongId_VerifyResponseIsNotFound()
        {
            // Arrange
            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(s => s.AddRouting().AddMicroService(b => b.AddJson()).BuildServiceProvider())
                .Configure(app => app.Route("users/{id}").Put(Program.Users).Use()));

            HttpClient client = server.CreateClient();
            
            // Act
            var response = await client.PutAsync($"/users/0", new StringContent("", Encoding.UTF8, "application/json"));

            string responseData = await response.Content.ReadAsStringAsync();
            
            // Assert
            response.StatusCode.Should().Be(404);
            responseData.Should().BeEmpty();
        }
        
        [Fact]
        public async Task StartServerWithPutMethod_PutUserWithDifferentIdOfBody_VerifyResponseIsBadRequest()
        {
            // Arrange
            var updatedUser = new User
            {
                FirstName = "Jhon",
                LastName = "Doe",
                Age = 42,
                Email = "jdoe@foo.bar",
                Id = Guid.NewGuid()
            };
            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(s => s.AddRouting().AddMicroService(b => b.AddJson()).BuildServiceProvider())
                .Configure(app => app.Route("users/{id}").Put(Program.Users).Use()));
            HttpClient client = server.CreateClient();
            
            // Act
            User user = CrudSample.Program.Users.First();
            var response = await client.PutAsync($"/users/{user.Id}", new StringContent(JsonConvert.SerializeObject(updatedUser), Encoding.UTF8, "application/json"));

            string responseData = await response.Content.ReadAsStringAsync();
            
            // Assert
            response.StatusCode.Should().Be(400);
            responseData.Should().BeEmpty();
        }
        
        [Fact]
        public async Task StartServerWithPutMethod_PutUser_VerifyUserIsUpdated()
        {
            // Arrange
            var updatedUser = new User
            {
                FirstName = "Jhoana",
                LastName = "Doe",
                Age = 1,
                Email = "jdoe@foo.bar",
            };
            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(s => s.AddRouting().AddMicroService(b => b.AddJson()).BuildServiceProvider())
                .Configure(app => app.Route("users/{id}").Put(Program.Users).Use()));
            HttpClient client = server.CreateClient();
            
            // Act
            User user = CrudSample.Program.Users.First();
            updatedUser.Id = user.Id;
            var response = await client.PutAsync($"/users/{user.Id}", new StringContent(JsonConvert.SerializeObject(updatedUser), Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            string responseData = await response.Content.ReadAsStringAsync();
            
            // Assert
            response.StatusCode.Should().Be(204);
            responseData.Should().BeEmpty();
            user.Age.Should().Be(1);
        }
    }
}