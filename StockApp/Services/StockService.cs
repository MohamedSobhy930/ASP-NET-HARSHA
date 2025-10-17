using IServicesContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class StockService : IStockService
    {
        private readonly List<BuyOrder> _buyOrders;
        private readonly List<SellOrder> _sellOrders;
        public StockService() 
        {
            _buyOrders = new List<BuyOrder>();
            _sellOrders = new List<SellOrder>();
        }
        public Task<BuyOrderResponse> CreateBuyOrder(BuyOrderRequest? buyOrderRequest)
        {
            if(buyOrderRequest == null)
                throw new ArgumentNullException(nameof(buyOrderRequest));
            ValidationHelper.ModelValidation(buyOrderRequest);
            BuyOrder order = buyOrderRequest.ToBuyOrder();
            order.BuyOrderID = Guid.NewGuid();
            _buyOrders.Add(order);
            return Task.FromResult(order.ToBuyOrderResponse());
        }

        public Task<SellOrderResponse> CreateSellOrder(SellOrderRequest? sellOrderRequest)
        {
            if(sellOrderRequest == null)
                throw new ArgumentNullException(nameof(sellOrderRequest));  
            ValidationHelper.ModelValidation(sellOrderRequest);
            SellOrder order = sellOrderRequest.ToSellOrder();
            order.SellOrderID = Guid.NewGuid();
            _sellOrders.Add(order);
            return Task.FromResult(order.ToSellOrderResponse());
        }

        public Task<List<BuyOrderResponse>> GetBuyOrders()
        {
            List<BuyOrderResponse> orders = _buyOrders
                .Select(order => order.ToBuyOrderResponse())
                .ToList();
            return Task.FromResult(orders);

        }

        public Task<List<SellOrderResponse>> GetSellOrders()
        {
            List<SellOrderResponse> orders = _sellOrders
                .Select(order => order.ToSellOrderResponse())
                .ToList();
            return Task.FromResult(orders);
        }
    }
}
