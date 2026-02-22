using FluentAssertions;

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
        }
        #endregion
    }
}
