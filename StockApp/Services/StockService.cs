using Entities;
using IServicesContracts;
using ReposContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class StockService : IStockService
    {
        private readonly IStocksRepo _stocksRepo;
        public StockService(IStocksRepo stocksRepo) 
        {
            _stocksRepo = stocksRepo;
        }
        public async Task<BuyOrderResponse> CreateBuyOrder(BuyOrderRequest? buyOrderRequest)
        {
            if(buyOrderRequest == null)
                throw new ArgumentNullException(nameof(buyOrderRequest));
            ValidationHelper.ModelValidation(buyOrderRequest);
            BuyOrder order = buyOrderRequest.ToBuyOrder();
            order.BuyOrderID = Guid.NewGuid();
            return (await _stocksRepo.CreateBuyOrder(order)).ToBuyOrderResponse();
        }

        public async Task<SellOrderResponse> CreateSellOrder(SellOrderRequest? sellOrderRequest)
        {
            if(sellOrderRequest == null)
                throw new ArgumentNullException(nameof(sellOrderRequest));  
            ValidationHelper.ModelValidation(sellOrderRequest);
            SellOrder order = sellOrderRequest.ToSellOrder();
            order.SellOrderID = Guid.NewGuid();
            return (await _stocksRepo.CreateSellOrder(order)).ToSellOrderResponse();
        }

        public async Task<List<BuyOrderResponse>> GetBuyOrders()
        {
            var orders = await _stocksRepo.GetBuyOrders();
            List<BuyOrderResponse> orderResponses = orders
                .Select(order => order.ToBuyOrderResponse())
                .ToList();
            return orderResponses;
        }

        public async Task<List<SellOrderResponse>> GetSellOrders()
        {
            var orders = await _stocksRepo.GetSellOrders();
            List<SellOrderResponse> orderResponses = orders
                .Select(order => order.ToSellOrderResponse())
                .ToList();
            return orderResponses;
        }
    }
}
