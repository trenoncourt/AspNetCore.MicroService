using System.Net.Http;
using System.Threading.Tasks;
using AspNetCore.MicroService.Routing.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
    }
}