using IServicesContracts;
using Microsoft.Extensions.Configuration;
using ReposContracts;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace StockAppWithConfigurationAssignment.Services
{
    public class FinnhubService : IFinnhubService
    {
        private readonly IFinnhubRepo _finnhubRepo;
        public FinnhubService(IFinnhubRepo finnhubRepo) 
        {
            _finnhubRepo = finnhubRepo;
        }
        public async Task<Dictionary<string, object>?> GetCompanyProfile(string stockSymbol)
        {
            return await _finnhubRepo.GetCompanyProfile(stockSymbol);
        }

        public async Task<Dictionary<string, object>?> GetStockPriceQuote(string stockSymbol)
        {
            return await _finnhubRepo.GetStockPriceQuote(stockSymbol);
        }

        public async Task<List<Dictionary<string, string>>?> GetStocks()
        {
            return await _finnhubRepo.GetStocks();
        }

        public async Task<Dictionary<string, object>?> SearchStocks(string stockSymbolToSearch)
        {
            return await _finnhubRepo.SearchStocks(stockSymbolToSearch);
        }
    }
}
