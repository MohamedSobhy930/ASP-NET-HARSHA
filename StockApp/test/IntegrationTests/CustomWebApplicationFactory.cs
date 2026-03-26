using Entities;
using Repos;
using IServicesContracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using StockAppWithConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.UseEnvironment("Test");
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IFinnhubService>();

                var mockFinnhubService = new Mock<IFinnhubService>();

                mockFinnhubService.Setup(temp => temp.GetCompanyProfile(It.IsAny<string>()))
                    .ReturnsAsync(new Dictionary<string, object>
                    {
                    { "name", "Microsoft Corp" },
                    { "logo", "https://example.com/logo.png" },
                    { "finnhubIndustry", "Technology" },
                    { "exchange", "NASDAQ" }
                    });

                mockFinnhubService.Setup(temp => temp.GetStockPriceQuote(It.IsAny<string>()))
                    .ReturnsAsync(new Dictionary<string, object>
                    {
                    { "c", 150.00 } 
                    });

                mockFinnhubService.Setup(temp => temp.GetStocks())
                   .ReturnsAsync(new List<Dictionary<string, string>>()
                   {
                     new Dictionary<string,string>() { { "symbol", "AAPL" }, { "description", "Apple Inc" } }
                   });

                services.AddTransient<IFinnhubService>(provider => mockFinnhubService.Object);

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestingDatabase");
                });
            });
        }
    }
}
