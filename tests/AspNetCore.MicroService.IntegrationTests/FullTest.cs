using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.MicroService.Extensions.Json;
using FluentAssertions;
using FullSample;
using FullSample.Dtos;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Xunit;

namespace AspNetCore.MicroService.IntegrationTests
{
    public class FullTest
    {
        [Fact]
        public async Task StartFullSampleServer_GetUsers_VerifyResultContainsUsers()
        {
            // Arrange
            var server = new TestServer(Program.CreateWebHostBuilder());

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
        public async Task StartFullSampleServer_PostUserGetLocationQueryLocation_VerifyUserAdded()
        {
            // Arrange
            var user = new User
            {
                FirstName = "Peter",
                LastName = "Mc Callow",
                Email = "pmccallow@xyz.bar",
                Age = 38,
            };
            var server = new TestServer(Program.CreateWebHostBuilder());

            HttpClient client = server.CreateClient();
            
            // Act
            var postResponse = await client.PostAsync("/users", new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));
            postResponse.EnsureSuccessStatusCode();
            string postResponseData = await postResponse.Content.ReadAsStringAsync();
            
            string location = postResponse.Headers.Location.ToString();
            string id = location.Replace("/users/", "");
                
            var locationResponse = await client.GetAsync(location);
            locationResponse.EnsureSuccessStatusCode();
            string locationResponseData = await locationResponse.Content.ReadAsStringAsync();
            User foundedUser = JsonConvert.DeserializeObject<User>(locationResponseData);
            
            // Assert
            postResponse.StatusCode.Should().Be(201);
            postResponseData.Should().BeEmpty();
            Program.Users.Should().ContainSingle(u => u.Id.ToString() == id);
            locationResponse.StatusCode.Should().Be(200);
            locationResponse.Content.Headers.ContentType.MediaType.Should().Be("application/json");
            Program.Users.Single(u => u.Id.ToString() == id).Should().BeEquivalentTo(foundedUser);
        }
        
        [Fact]
        public async Task StartFullSampleServer_PostUserWithWrongObjectType_VerifyResponseIsBadRequest()
        {
            // Arrange
            string badJson = "";
            var server = new TestServer(Program.CreateWebHostBuilder());

            HttpClient client = server.CreateClient();
            
            // Act
            var postResponse = await client.PostAsync("/users", new StringContent(badJson, Encoding.UTF8, "application/json"));
            string postResponseData = await postResponse.Content.ReadAsStringAsync();
            
            // Assert
            postResponse.StatusCode.Should().Be(400);
            postResponseData.Should().BeEmpty();
        }
        
        [Fact]
        public async Task StartFullSampleServer_GetUserRoute_VerifyResultContainsTheUser()
        {
            // Arrange
            var server = new TestServer(Program.CreateWebHostBuilder());

            HttpClient client = server.CreateClient();
            
            // Act
            User user = Program.Users.First();
            var response = await client.GetAsync($"/users/{user.Id}");
            response.EnsureSuccessStatusCode();

            string responseData = await response.Content.ReadAsStringAsync();
            
            // Assert
            response.StatusCode.Should().Be(200);
            response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
            responseData.Should().Be(JsonConvert.SerializeObject(user, JsonSerializerSettingsProvider.CreateSerializerSettings()));
        }
        
        [Fact]
        public async Task StartFullSampleServer_GetUserWithWrongId_VerifyResponseIsNotFound()
        {
            // Arrange
            var server = new TestServer(Program.CreateWebHostBuilder());

            HttpClient client = server.CreateClient();
            
            // Act
            var response = await client.GetAsync($"/users/0");

            string responseData = await response.Content.ReadAsStringAsync();
            
            // Assert
            response.StatusCode.Should().Be(404);
            responseData.Should().BeEmpty();
        }
        
        [Fact]
        public async Task StartFullSampleServer_PutUserWithWrongId_VerifyResponseIsNotFound()
        {
            // Arrange
            var server = new TestServer(Program.CreateWebHostBuilder());

            HttpClient client = server.CreateClient();
            
            // Act
            var response = await client.PutAsync($"/users/0", new StringContent("", Encoding.UTF8, "application/json"));

            string responseData = await response.Content.ReadAsStringAsync();
            
            // Assert
            response.StatusCode.Should().Be(404);
            responseData.Should().BeEmpty();
        }
        
        [Fact]
        public async Task StartFullSampleServer_PutUserWithDifferentIdOfBody_VerifyResponseIsBadRequest()
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
            var server = new TestServer(Program.CreateWebHostBuilder());
            HttpClient client = server.CreateClient();
            
            // Act
            User user = Program.Users.First();
            var response = await client.PutAsync($"/users/{user.Id}", new StringContent(JsonConvert.SerializeObject(updatedUser), Encoding.UTF8, "application/json"));

            string responseData = await response.Content.ReadAsStringAsync();
            
            // Assert
            response.StatusCode.Should().Be(400);
            responseData.Should().BeEmpty();
        }
        
        [Fact]
        public async Task StartFullSampleServer_PutUser_VerifyUserIsUpdated()
        {
            // Arrange
            var updatedUser = new User
            {
                FirstName = "Jhoana",
                LastName = "Doe",
                Age = 1,
                Email = "jdoe@foo.bar",
            };
            var server = new TestServer(Program.CreateWebHostBuilder());
            HttpClient client = server.CreateClient();
            
            // Act
            User user = Program.Users.First();
            updatedUser.Id = user.Id;
            var response = await client.PutAsync($"/users/{user.Id}", new StringContent(JsonConvert.SerializeObject(updatedUser), Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            string responseData = await response.Content.ReadAsStringAsync();
            
            // Assert
            response.StatusCode.Should().Be(204);
            responseData.Should().BeEmpty();
            user.Age.Should().Be(1);
        }
        
        [Fact]
        public async Task StartFullSampleServer_DeleteUser_VerifyUserIsDeleted()
        {
            // Arrange
            var server = new TestServer(Program.CreateWebHostBuilder());
            HttpClient client = server.CreateClient();
            
            // Act
            User user = Program.Users.First();
            var response = await client.DeleteAsync($"/users/{user.Id}");
            response.EnsureSuccessStatusCode();

            string responseData = await response.Content.ReadAsStringAsync();
            
            // Assert
            response.StatusCode.Should().Be(204);
            responseData.Should().BeEmpty();
            Program.Users.FirstOrDefault(u => u.Id == user.Id).Should().BeNull();
        }
        
        [Fact]
        public async Task StartFullSampleServer_DeleteUser_VerifyResponseIsNotFound()
        {
            // Arrange
            var server = new TestServer(Program.CreateWebHostBuilder());

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