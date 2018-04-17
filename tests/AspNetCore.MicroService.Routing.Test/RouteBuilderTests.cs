using System.Net.Http;
using System.Threading.Tasks;
using AspNetCore.MicroService.Routing.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AspNetCore.MicroService.Routing.Test
{
    public class RouteBuilderTests
    {
        [Fact]
        public async Task Get_AddGetTestsRoute_VerifyResultIsTest()
        {
            // Arrange
            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(s => s.AddRouting().BuildServiceProvider())
                .Configure(app => app.Route("tests").Get(async c => await c.Response.WriteAsync("test")).Use()));

            HttpClient client = server.CreateClient();
            
            // Act
            var response = await client.GetAsync("/tests");
            response.EnsureSuccessStatusCode();

            string responseData = await response.Content.ReadAsStringAsync();
            
            // Assert
            Assert.Equal("test", responseData);
        }
        
        [Fact]
        public async Task Post_AddPostTestsRoute_VerifyResultIsTest()
        {
            // Arrange
            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(s => s.AddRouting().BuildServiceProvider())
                .Configure(app => app.Route("tests").Post(async c => await c.Response.WriteAsync("test")).Use()));

            HttpClient client = server.CreateClient();
            
            // Act
            var response = await client.PostAsync("/tests", null);
            response.EnsureSuccessStatusCode();

            string responseData = await response.Content.ReadAsStringAsync();
            
            // Assert
            Assert.Equal("test", responseData);
        }
        
        [Fact]
        public async Task Post_AddPostTestsRoute_VerifyResultEqualsBody()
        {
            // Arrange
            string test = "test";
            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(s => s.AddRouting().BuildServiceProvider())
                .Configure(app => app.Route("tests").Post(async c =>
                {
                    c.Request.EnableRewind();
                    await c.Request.Body.CopyToAsync(c.Response.Body);
                }).Use()));

            HttpClient client = server.CreateClient();
            
            // Act
            var response = await client.PostAsync("/tests", new StringContent(test));
            response.EnsureSuccessStatusCode();

            string responseData = await response.Content.ReadAsStringAsync();
            
            // Assert
            Assert.Equal(test, responseData);
        }
    }
}