using System.Linq;
using System.Net.Http;
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
using Xunit;

namespace AspNetCore.MicroService.Extensions.Crud.Tests
{
    public class DeleteTest
    {
        [Fact]
        public async Task StartServerWithDeleteMethod_DeleteUser_VerifyUserIsDeleted()
        {
            // Arrange
            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(s => s.AddRouting().AddMicroService(b => b.AddJson()).BuildServiceProvider())
                .Configure(app => app.Route("users/{id}").Delete(Program.Users).Use()));
            HttpClient client = server.CreateClient();
            
            // Act
            User user = CrudSample.Program.Users.First();
            var response = await client.DeleteAsync($"/users/{user.Id}");
            response.EnsureSuccessStatusCode();

            string responseData = await response.Content.ReadAsStringAsync();
            
            // Assert
            response.StatusCode.Should().Be(204);
            responseData.Should().BeEmpty();
            CrudSample.Program.Users.FirstOrDefault(u => u.Id == user.Id).Should().BeNull();
        }
        
        [Fact]
        public async Task StartServerWithDeleteMethod_DeleteUser_VerifyResponseIsNotFound()
        {
            // Arrange
            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(s => s.AddRouting().AddMicroService(b => b.AddJson()).BuildServiceProvider())
                .Configure(app => app.Route("users/{id}").Delete(Program.Users).Use()));

            HttpClient client = server.CreateClient();
            
            // Act
            var response = await client.DeleteAsync($"/users/0");

            string responseData = await response.Content.ReadAsStringAsync();
            
            // Assert
            response.StatusCode.Should().Be(404);
            responseData.Should().BeEmpty();
        } 
    }
}