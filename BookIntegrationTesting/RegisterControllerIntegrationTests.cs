using BookClassLib.Models;
using BookWebapi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http.Json;

namespace BookIntegrationTesting
{
    public class RegisterControllerIntegrationTests : IClassFixture<TestingWebAppFactory<Startup>>
    {
        private readonly HttpClient _client;
        public RegisterControllerIntegrationTests(TestingWebAppFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }
        

        [Fact]
        public async Task CanGetBooks()
        {
            // The endpoint or route of the controller action.
            var httpResponse = await _client.GetAsync("/api/BookAPI");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var players = JsonConvert.DeserializeObject<IEnumerable<Book>>(stringResponse);
            Assert.Contains(players, p => p.Author == "venkat");
            Assert.Contains(players, p => p.Name == "somerandom");
        }

        [Fact]
        public async Task CanGetBookByID()
        {
            var id = 1;
            // The endpoint or route of the controller action.
            var httpResponse = await _client.GetAsync($"/api/BookAPI/{id}");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var player = JsonConvert.DeserializeObject<Book>(stringResponse);
            Assert.Equal("venkat", player.Author);
            Assert.Equal("somerandom", player.Name);           
        }

        [Fact]
        public async Task PostAction()
        {
            var BookObj = new Book() { Author = "Special", BookNo = 4, Name = "Random", Publisher = "New" };
            var id = 1;
            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsJsonAsync<Book>("api/BookAPI", BookObj);

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var player = JsonConvert.DeserializeObject<Book>(stringResponse);
            Assert.Equal("Special", player.Author);
            Assert.Equal("Random", player.Name);
        }

        [Fact]
        public async Task PostActionInvalid()
        {
            var BookObj = new Book() { Author = "Special", BookNo = 5, Publisher = "New" };           
            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsJsonAsync<Book>("api/BookAPI", BookObj);


            var statuscode = httpResponse.StatusCode.ToString();
            Assert.Contains("BadRequest", statuscode);
        }

        [Fact]
        public async Task PutMethodTest()
        {
            var BookObj = new Book() { Author = "Special", BookNo = 1, Name = "Random", Publisher = "New" };
            // The endpoint or route of the controller action.
            var httpResponse = await _client.PutAsJsonAsync<Book>("api/BookAPI", BookObj);

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();
            var statuscode = httpResponse.StatusCode.ToString();
            Assert.Contains("NoContent", statuscode); 
        }

        [Fact]
        public async Task DeleteMethodTest()
        {
            int id = 2;
            var httpResponse = await _client.DeleteAsync($"api/BookAPI/{id}");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();
            var statuscode = httpResponse.StatusCode.ToString();
            Assert.Contains("NoContent", statuscode);
        }
    }
}
