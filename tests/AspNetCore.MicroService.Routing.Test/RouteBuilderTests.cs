using System.Net.Http;
using System.Threading.Tasks;
using AspNetCore.MicroService.Routing.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AspNetCore.MicroService.Routing.Test
{
    public class RouteBuilderTests
    {
        [Fact]
        public async Task AddGetTestsRoute_QueryRoute_VerifyResultIsTest()
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
        
        [Theory]
        [InlineData("1")]
        [InlineData("a")]
        public async Task AddRouteWithParameter_QueryRoute_VerifyResultIsParameter(string parameter)
        {
            // Arrange
            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(s => s.AddRouting().BuildServiceProvider())
                .Configure(app => app.Route("tests/{id}").Get(async c =>
                {
                    await c.Response.WriteAsync(c.GetRouteData().Values["id"].ToString());
                }).Use()));

            HttpClient client = server.CreateClient();
            
            // Act
            var response = await client.GetAsync($"/tests/{parameter}");
            response.EnsureSuccessStatusCode();

            string responseData = await response.Content.ReadAsStringAsync();
            
            // Assert
            Assert.Equal(parameter, responseData);
        }
        
        [Fact]
        public async Task AddPostTestsRoute_QueryRoute_VerifyResultIsTest()
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
        public async Task AddPostRouteReturnsBody_QueryRoute_VerifyResultEqualsBody()
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
        
        [Fact]
        public async Task AddPutTestsRoute_QueryRoute_VerifyResultIsTest()
        {
            // Arrange
            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(s => s.AddRouting().BuildServiceProvider())
                .Configure(app => app.Route("tests").Put(async c => await c.Response.WriteAsync("test")).Use()));

            HttpClient client = server.CreateClient();
            
            // Act
            var response = await client.PutAsync("/tests", null);
            response.EnsureSuccessStatusCode();

            string responseData = await response.Content.ReadAsStringAsync();
            
            // Assert
            Assert.Equal("test", responseData);
        }
        
        [Fact]
        public async Task AddPutRouteReturnsBody_QueryRoute_VerifyResultEqualsBody()
        {
            // Arrange
            string test = "test";
            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(s => s.AddRouting().BuildServiceProvider())
                .Configure(app => app.Route("tests").Put(async c =>
                {
                    c.Request.EnableRewind();
                    await c.Request.Body.CopyToAsync(c.Response.Body);
                }).Use()));

            HttpClient client = server.CreateClient();
            
            // Act
            var response = await client.PutAsync("/tests", new StringContent(test));
            response.EnsureSuccessStatusCode();

            string responseData = await response.Content.ReadAsStringAsync();
            
            // Assert
            Assert.Equal(test, responseData);
        }
        
        [Fact]
        public async Task AddDeleteTestsRoute_QueryRoute_VerifyResultIsTest()
        {
            // Arrange
            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(s => s.AddRouting().BuildServiceProvider())
                .Configure(app => app.Route("tests").Delete(async c => await c.Response.WriteAsync("test")).Use()));

            HttpClient client = server.CreateClient();
            
            // Act
            var response = await client.DeleteAsync("/tests");
            response.EnsureSuccessStatusCode();

            string responseData = await response.Content.ReadAsStringAsync();
            
            // Assert
            Assert.Equal("test", responseData);
        }
        
        [Fact]
        public async Task AddTwoRoutesWithDifferentResults_QueryRoutes_VerifyResults()
        {
            // Arrange
            string res1 = "test 1";
            string res2 = "test 2";
            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(s => s.AddRouting().BuildServiceProvider())
                .Configure(app => 
                    app.Route("tests1")
                        .Get(async c => await c.Response.WriteAsync(res1))
                    .Route("tests2")
                        .Get(async c => await c.Response.WriteAsync(res2))    
                    .Use()));

            HttpClient client = server.CreateClient();
            
            // Act
            var response1 = await client.GetAsync("/tests1");
            response1.EnsureSuccessStatusCode();
            
            var response2 = await client.GetAsync("/tests2");
            response2.EnsureSuccessStatusCode();

            string responseData1 = await response1.Content.ReadAsStringAsync();
            string responseData2 = await response2.Content.ReadAsStringAsync();
            
            // Assert
            Assert.Equal(res1, responseData1);
            Assert.Equal(res2, responseData2);
        }
        
        [Fact]
        public async Task AddOneRouteWithOneSubRoute_QueryRoutes_VerifyResults()
        {
            // Arrange
            string res1 = "test 1";
            string res2 = "test 2";
            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(s => s.AddRouting().BuildServiceProvider())
                .Configure(app => 
                    app.Route("tests")
                        .Get(async c => await c.Response.WriteAsync(res1))
                    .Route("tests/tests")
                        .Get(async c => await c.Response.WriteAsync(res2))    
                    .Use()));

            HttpClient client = server.CreateClient();
            
            // Act
            var response1 = await client.GetAsync("/tests");
            response1.EnsureSuccessStatusCode();
            
            var response2 = await client.GetAsync("/tests/tests");
            response2.EnsureSuccessStatusCode();

            string responseData1 = await response1.Content.ReadAsStringAsync();
            string responseData2 = await response2.Content.ReadAsStringAsync();
            
            // Assert
            Assert.Equal(res1, responseData1);
            Assert.Equal(res2, responseData2);
        }
        
        [Fact]
        public async Task AddSubRouteWithQuery_QueryRoutes_VerifyResults()
        {
            // Arrange
            string res1 = "test 1";
            string res2 = "test 2";
            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(s => s.AddRouting().BuildServiceProvider())
                .Configure(app => 
                    app.Route("tests")
                        .Get(async c => await c.Response.WriteAsync(res1))
                    .Route("tests/{id}")
                        .Get(async c => await c.Response.WriteAsync(res2))    
                    .Use()));

            HttpClient client = server.CreateClient();
            
            // Act
            var response1 = await client.GetAsync("/tests");
            response1.EnsureSuccessStatusCode();
            
            var response2 = await client.GetAsync("/tests/1");
            response2.EnsureSuccessStatusCode();

            string responseData1 = await response1.Content.ReadAsStringAsync();
            string responseData2 = await response2.Content.ReadAsStringAsync();
            
            // Assert
            Assert.Equal(res1, responseData1);
            Assert.Equal(res2, responseData2);
        }
        
    }
}