using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReposContracts
{
    public interface IStocksRepo
    {
        Task<BuyOrder> CreateBuyOrder(BuyOrder buyOrder);

        Task<SellOrder> CreateSellOrder(SellOrder sellOrder);

        Task<List<BuyOrder>> GetBuyOrders();

        Task<List<SellOrder>> GetSellOrders();
    }
}
