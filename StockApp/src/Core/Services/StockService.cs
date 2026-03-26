using Entities;
using IServicesContracts;
using Microsoft.Extensions.Logging;
using ReposContracts;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class StockService(ILogger<StockService> logger, IStocksRepo stocksRepo) : IStockService
    {
        private readonly IStocksRepo _stocksRepo = stocksRepo;
        private readonly ILogger<StockService> _logger = logger;

        public async Task<BuyOrderResponse> CreateBuyOrder(BuyOrderRequest? buyOrderRequest)
        {
            _logger.LogInformation("Creating a new buy order.");
            if (buyOrderRequest == null)
                throw new ArgumentNullException(nameof(buyOrderRequest));
            ValidationHelper.ModelValidation(buyOrderRequest);
            BuyOrder order = buyOrderRequest.ToBuyOrder();
            order.BuyOrderID = Guid.NewGuid();
            return (await _stocksRepo.CreateBuyOrder(order)).ToBuyOrderResponse();
        }

        public async Task<SellOrderResponse> CreateSellOrder(SellOrderRequest? sellOrderRequest)
        {
            _logger.LogInformation("Creating a new sell order.");
            if (sellOrderRequest == null)
                throw new ArgumentNullException(nameof(sellOrderRequest));  
            ValidationHelper.ModelValidation(sellOrderRequest);
            SellOrder order = sellOrderRequest.ToSellOrder();
            order.SellOrderID = Guid.NewGuid();
            return (await _stocksRepo.CreateSellOrder(order)).ToSellOrderResponse();
        }

        public async Task<List<BuyOrderResponse>> GetBuyOrders()
        {
            _logger.LogInformation("Retrieving all buy orders.");
            var orders = await _stocksRepo.GetBuyOrders();
            List<BuyOrderResponse> orderResponses = orders
                .Select(order => order.ToBuyOrderResponse())
                .ToList();
            return orderResponses;
        }

        public async Task<List<SellOrderResponse>> GetSellOrders()
        {
            _logger.LogInformation("Retrieving all sell orders.");
            var orders = await _stocksRepo.GetSellOrders();
            List<SellOrderResponse> orderResponses = orders
                .Select(order => order.ToSellOrderResponse())
                .ToList();
            return orderResponses;
        }
    }
}
