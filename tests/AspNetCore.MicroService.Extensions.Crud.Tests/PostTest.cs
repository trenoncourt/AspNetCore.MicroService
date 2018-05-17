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
    public class PostTest
    {
        [Fact]
        public async Task StartServerWithPostMethod_PostUserGetLocationQueryLocation_VerifyUserAdded()
        {
            // Arrange
            var user = new User
            {
                FirstName = "Peter",
                LastName = "Mc Callow",
                Email = "pmccallow@xyz.bar",
                Age = 38,
            };
            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(s => s.AddRouting().AddMicroService(b => b.AddJson()).BuildServiceProvider())
                .Configure(app => app.Route("users").Post(Program.Users).Use()));

            HttpClient client = server.CreateClient();
            
            // Act
            var postResponse = await client.PostAsync("/users", new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));
            postResponse.EnsureSuccessStatusCode();
            string postResponseData = await postResponse.Content.ReadAsStringAsync();
            
            // Assert
            postResponse.StatusCode.Should().Be(201);
            postResponseData.Should().BeEmpty();
        }
        
        [Fact]
        public async Task StartServerWithPostMethod_PostUserWithWrongObjectType_VerifyResponseIsBadRequest()
        {
            // Arrange
            string badJson = "";
            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(s => s.AddRouting().AddMicroService(b => b.AddJson()).BuildServiceProvider())
                .Configure(app => app.Route("users").Post(Program.Users).Use()));

            HttpClient client = server.CreateClient();
            
            // Act
            var postResponse = await client.PostAsync("/users", new StringContent(badJson, Encoding.UTF8, "application/json"));
            string postResponseData = await postResponse.Content.ReadAsStringAsync();
            
            // Assert
            postResponse.StatusCode.Should().Be(400);
            postResponseData.Should().BeEmpty();
        }
    }
}