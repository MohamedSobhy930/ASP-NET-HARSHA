using FluentAssertions;
using System.Net.Http;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xUnitTests
{
    public class PersonsControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        public PersonsControllerIntegrationTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }
        #region Index
        [Fact]
        public async Task Index_ReturnView()
        {
            // arrange

            // act
            HttpResponseMessage responseMessage =await _client.GetAsync("/Persons/Index");

            // assert
            string responseBody = await responseMessage.Content.ReadAsStringAsync();
            responseMessage.IsSuccessStatusCode.Should().BeTrue(
                $"the server responded with {responseMessage.StatusCode}. Response body: {responseBody}");

            responseMessage.Content.Headers.ContentType.ToString().Should().Contain("text/html");
            //responseBody.Should().Contain("<h1>Persons List</h1>");
        }
        #endregion
    }
}
