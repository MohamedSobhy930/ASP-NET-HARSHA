using AutoFixture;
using FluentAssertions;
using IServicesContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using ReposContracts;
using Services;
using StockAppAssignment.Controllers;
using StockAppAssignment.Models;
using StockAppWithConfigurationAssignment.Models;
using StockAppWithConfigurationAssignment.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class StocksControllerTest
    {
        private readonly IFinnhubService _finnhubService;
        // mock the repo 
        private readonly Mock<IFinnhubRepo> _finnhubRepositoryMock;
        private readonly IFixture _fixture;

        public StocksControllerTest()
        {
            _fixture = new Fixture();
            _finnhubRepositoryMock = new Mock<IFinnhubRepo>();
            _finnhubService = new FinnhubService(_finnhubRepositoryMock.Object);
        }
        [Fact]
        public async Task Explore_StockIsNull_ShouldReturnExploreViewWithStocksList()
        {
            // arrange 
            IOptions<TradingOptions> tradingOptions = Options.Create(new TradingOptions() 
            { 
                DefaultOrderQuantity = 100, 
                Top25PopularStocks = "AAPL,MSFT,AMZN,TSLA,GOOGL,GOOG,NVDA" 
            });

            StocksController stocksController = new StocksController(tradingOptions, _finnhubService);

            List<Dictionary<string, string>>? stocksDict = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string> { {"symbol", "AAPL"}, {"description", "APPLE INC"} },
                new Dictionary<string, string> { {"symbol", "MSFT"}, {"description", "MICROSOFT CORP"} },
                new Dictionary<string, string> { {"symbol", "AMZN"}, {"description", "AMAZON.COM INC"} },
                new Dictionary<string, string> { {"symbol", "TSLA"}, {"description", "TESLA INC"} },
                new Dictionary<string, string> { {"symbol", "GOOGL"}, {"description", "ALPHABET INC-CL A"} }
            };

            _finnhubRepositoryMock.Setup(temp => temp.GetStocks())
                .ReturnsAsync(stocksDict);

            var expectedStocks = stocksDict!
                .Select(temp => new Stock() 
                { StockName = temp["description"], StockSymbol = temp["symbol"] })
                .ToList();

            // act 
            var result = await stocksController.Explore(null);

            // assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Stock>>(viewResult.ViewData.Model);
            model.Should().BeEquivalentTo(expectedStocks);
        }
    }
}
