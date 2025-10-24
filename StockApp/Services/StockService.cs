using Entities;
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
        private AppDbContext _db;
        public StockService(AppDbContext db) 
        {
            _db = db;
        }
        public Task<BuyOrderResponse> CreateBuyOrder(BuyOrderRequest? buyOrderRequest)
        {
            if(buyOrderRequest == null)
                throw new ArgumentNullException(nameof(buyOrderRequest));
            ValidationHelper.ModelValidation(buyOrderRequest);
            BuyOrder order = buyOrderRequest.ToBuyOrder();
            order.BuyOrderID = Guid.NewGuid();
            _db.BuyOrders.Add(order);
            _db.SaveChanges();
            return Task.FromResult(order.ToBuyOrderResponse());
        }

        public Task<SellOrderResponse> CreateSellOrder(SellOrderRequest? sellOrderRequest)
        {
            if(sellOrderRequest == null)
                throw new ArgumentNullException(nameof(sellOrderRequest));  
            ValidationHelper.ModelValidation(sellOrderRequest);
            SellOrder order = sellOrderRequest.ToSellOrder();
            order.SellOrderID = Guid.NewGuid();
            _db.SellOrders.Add(order);
            _db.SaveChanges();
            return Task.FromResult(order.ToSellOrderResponse());
        }

        public Task<List<BuyOrderResponse>> GetBuyOrders()
        {
            List<BuyOrderResponse> orders = _db.BuyOrders
                .Select(order => order.ToBuyOrderResponse())
                .ToList();
            return Task.FromResult(orders);

        }

        public Task<List<SellOrderResponse>> GetSellOrders()
        {
            List<SellOrderResponse> orders = _db.SellOrders
                .Select(order => order.ToSellOrderResponse())
                .ToList();
            return Task.FromResult(orders);
        }
    }
}
