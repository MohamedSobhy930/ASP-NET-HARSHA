using IServicesContracts;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace StockAppWithConfigurationAssignment.Services
{
    public class FinnhubService : IFinnhubService
    {
        private readonly IConfiguration _configuration;
        private IHttpClientFactory _httpClientFactory;
        public FinnhubService(IConfiguration configuration,
            IHttpClientFactory httpClientFactory) 
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<Dictionary<string, object>?> GetCompanyProfile(string stockSymbol)
        {
            using (HttpClient httpClient = _httpClientFactory.CreateClient())
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage()
                {
                    RequestUri = new Uri($"https://finnhub.io/api/v1/stock/profile2?symbol={stockSymbol}&token={_configuration["FinnhubToken"]}"),
                    Method = HttpMethod.Get
                };
                HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
                Stream stream = httpResponseMessage.Content.ReadAsStream();

                StreamReader reader = new StreamReader(stream);
                string responseBody = reader.ReadToEnd();

                Dictionary<string, object>? responseDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
                if (responseDictionary == null)
                    throw new InvalidOperationException("no content for this stock");
                if (responseDictionary.ContainsKey("error"))
                    throw new InvalidOperationException($"{responseDictionary["error"]}");
                return responseDictionary;
            }
        }

        public async Task<Dictionary<string, object>?> GetStockPriceQuote(string stockSymbol)
        {
            using (HttpClient httpClient = _httpClientFactory.CreateClient())
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage()
                {
                    RequestUri = new Uri($"https://finnhub.io/api/v1/quote?symbol={stockSymbol}&token={_configuration["FinnhubToken"]}"),
                    Method = HttpMethod.Get
                };
                HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
                Stream stream = httpResponseMessage.Content.ReadAsStream();

                StreamReader reader = new StreamReader(stream);
                string responseBody = reader.ReadToEnd();

                Dictionary<string, object>? responseDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
                if (responseDictionary == null)
                    throw new InvalidOperationException("no content for this stock");
                if (responseDictionary.ContainsKey("error"))
                    throw new InvalidOperationException($"{responseDictionary["error"]}");
                return responseDictionary;
            }
        }
    }
}
