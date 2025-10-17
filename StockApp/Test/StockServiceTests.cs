using FluentAssertions;
using IServicesContracts;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Test
{
    public class StockServiceTests
    {
        private readonly IStockService _stockService;
        public StockServiceTests() 
        {
            _stockService = new StockService();
        }
        // this a helper method for a valid request to avoid repeating the code 
        private BuyOrderRequest CreateValidBuyOrderRequest()
        {
            return new BuyOrderRequest
            {
                StockSymbol = "MSFT",
                StockName = "Microsoft",
                DateAndTimeOfOrder = new DateTime(2025, 10, 17),
                Quantity = 100,
                Price = 150.0
            };
        }
        private SellOrderRequest CreateValidSellOrderRequest()
        {
            return new SellOrderRequest
            {
                StockSymbol = "AAPL",
                StockName = "Apple Inc.",
                DateAndTimeOfOrder = new DateTime(2025, 10, 17),
                Quantity = 50,
                Price = 200.0
            };
        }
        /*

         */
        #region CreateBuyOrder
        //1. When you supply BuyOrderRequest as null, it should throw ArgumentNullException.
        [Fact]
        public async Task CreateBuyOrder_NullRequest_ThrowsArgumentNullException()
        {
            // arrange 
            BuyOrderRequest? request  = null;
            // act 

            // assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                // act
                await _stockService.CreateBuyOrder(request);
            }
            );
        }
        //2. When you supply buyOrderQuantity as 0 (as per the specification, minimum is 1), it should throw ArgumentException.
        [Fact]
        public async Task CreateBuyOrder_QuantityisZero_ThrowsArgumentException()
        {
            // arrange 
            BuyOrderRequest request = CreateValidBuyOrderRequest();
            request.Quantity = 0;

            // assert 
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // act 
                await _stockService.CreateBuyOrder(request);
            });
        }
        //3. When you supply buyOrderQuantity as 100001 (as per the specification, maximum is 100000), it should throw ArgumentException.
        [Fact]
        public async Task CreateBuyOrder_QuantityIsBiggerThan100000_ThrowsArgumentException()
        {
            // arrange 
            BuyOrderRequest request = CreateValidBuyOrderRequest();
            request.Quantity = 100001;

            // assert 
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // act 
                await _stockService.CreateBuyOrder(request);
            });
        }
        //4. When you supply buyOrderPrice as 0 (as per the specification, minimum is 1), it should throw ArgumentException.
        [Fact]
        public async Task CreateBuyOrder_PriceIsZero_ThrowsArgumentException()
        {
            // arrange 
            BuyOrderRequest request = CreateValidBuyOrderRequest();
            request.Price = 0;

            // assert 
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // act 
                await _stockService.CreateBuyOrder(request);
            });
        }
        //5. When you supply buyOrderPrice as 10001 (as per the specification, maximum is 10000), it should throw ArgumentException.
        [Fact]
        public async Task CreateBuyOrder_PriceIsTooHigh_ThrowsArgumentException()
        {
            // arrange 
            BuyOrderRequest request = CreateValidBuyOrderRequest();
            request.Price = 10001;

            // assert 
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // act 
                await _stockService.CreateBuyOrder(request);
            });
        }
        //6. When you supply stock symbol=null (as per the specification, stock symbol can't be null), it should throw ArgumentException.
        [Fact]
        public async Task CreateBuyOrder_SymbolIsNull_ThrowsArgumentException()
        {
            // arrange 
            BuyOrderRequest buyOrderRequest = CreateValidBuyOrderRequest();
            buyOrderRequest.StockSymbol = null;

            // assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // act 
                await _stockService.CreateBuyOrder(buyOrderRequest);
            });
        }
        //7. When you supply dateAndTimeOfOrder as "1999-12-31" (YYYY-MM-DD) - (as per the specification, it should be equal or newer date than 2000-01-01), it should throw ArgumentException.
        [Fact]
        public async Task CreateBuyOrder_DateAndTimeOfOrderisWrong_ThrowsArgumentException()
        {
            // arrange 
            BuyOrderRequest buyOrderRequest = CreateValidBuyOrderRequest();
            buyOrderRequest.DateAndTimeOfOrder = DateTime.Parse("1999-12-31");

            // assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // act 
                await _stockService.CreateBuyOrder(buyOrderRequest);
            });
        }
        //8. If you supply all valid values, it should be successful and return an object of BuyOrderResponse type with auto-generated BuyOrderID(guid).
        [Fact]
        public async Task CreateBuyOrder_ProperOrder_ReturnBuyOrderResponse()
        {
            // arrange
            BuyOrderRequest request = CreateValidBuyOrderRequest();
            
            // act
            BuyOrderResponse response = await _stockService.CreateBuyOrder(request);

            // assert
            Assert.NotNull(response);
            Assert.IsType<BuyOrderResponse>(response);
            Assert.NotEqual(Guid.Empty, response.BuyOrderID);
            Assert.Equal(request.StockSymbol, response.StockSymbol);
            Assert.Equal(request.Quantity * request.Price, response.TradeAmount);
        }
        #endregion

        #region CreateSellOrder
        // 1. When you supply SellOrderRequest as null, it should throw ArgumentNullException.
        [Fact]
        public async Task CreateSellOrder_NullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            SellOrderRequest? request = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _stockService.CreateSellOrder(request);
            });
        }
        // 2. When you supply sellOrderQuantity as 0, it should throw ArgumentException.
        [Fact]
        public async Task CreateSellOrder_QuantityIsZero_ThrowsArgumentException()
        {
            // Arrange
            SellOrderRequest request = CreateValidSellOrderRequest();
            request.Quantity = 0;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stockService.CreateSellOrder(request);
            });
        }
        // 3. When you supply sellOrderQuantity as 100001, it should throw ArgumentException.
        [Fact]
        public async Task CreateSellOrder_QuantityTooHigh_ThrowsArgumentException()
        {
            // Arrange
            SellOrderRequest request = CreateValidSellOrderRequest();
            request.Quantity = 100001;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stockService.CreateSellOrder(request);
            });
        }
        // 4. When you supply sellOrderPrice as 0, it should throw ArgumentException.
        [Fact]
        public async Task CreateSellOrder_PriceIsZero_ThrowsArgumentException()
        {
            // Arrange
            SellOrderRequest request = CreateValidSellOrderRequest();
            request.Price = 0;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stockService.CreateSellOrder(request);
            });
        }
        // 5. When you supply sellOrderPrice as 10001, it should throw ArgumentException.
        [Fact]
        public async Task CreateSellOrder_PriceTooHigh_ThrowsArgumentException()
        {
            // Arrange
            SellOrderRequest request = CreateValidSellOrderRequest();
            request.Price = 10001;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stockService.CreateSellOrder(request);
            });
        }
        // 6. When you supply stock symbol=null, it should throw ArgumentException.
        [Fact]
        public async Task CreateSellOrder_NullStockSymbol_ThrowsArgumentException()
        {
            // Arrange
            SellOrderRequest request = CreateValidSellOrderRequest();
            request.StockSymbol = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stockService.CreateSellOrder(request);
            });
        }
        // 7. When you supply dateAndTimeOfOrder as "1999-12-31", it should throw ArgumentException.
        [Fact]
        public async Task CreateSellOrder_DateTooOld_ThrowsArgumentException()
        {
            // Arrange
            SellOrderRequest request = CreateValidSellOrderRequest();
            request.DateAndTimeOfOrder = new DateTime(1999, 12, 31);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stockService.CreateSellOrder(request);
            });
        }
        // 8. If you supply all valid values, it should be successful and return SellOrderResponse.
        [Fact]
        public async Task CreateSellOrder_ValidRequest_ReturnsSellOrderResponse()
        {
            // Arrange
            SellOrderRequest request = CreateValidSellOrderRequest();

            // Act
            SellOrderResponse response = await _stockService.CreateSellOrder(request);

            // Assert
            Assert.NotNull(response);
            Assert.IsType<SellOrderResponse>(response);
            Assert.NotEqual(Guid.Empty, response.SellOrderID);
            Assert.Equal(request.StockSymbol, response.StockSymbol);
            Assert.Equal(request.Quantity * request.Price, response.TradeAmount);
        }
        #endregion

        #region GetAllBuyOrders
        [Fact]
        public async Task GetAllBuyOrders_DefaultList_ReturnEmpty()
        {
            // arrange

            // act
            var response =await _stockService.GetBuyOrders();

            // assert
            Assert.NotNull(response);
            Assert.Empty(response);
        }
        [Fact]
        public async Task GetAllBuyOrders_OrdersFound_ReturnList()
        {
            // arrange
            BuyOrderRequest request1 = CreateValidBuyOrderRequest();
            request1.StockSymbol = "MSFT";
            BuyOrderRequest request2 = CreateValidBuyOrderRequest();
            request1.StockSymbol = "GOOG";

            await _stockService.CreateBuyOrder(request1);
            await _stockService.CreateBuyOrder(request2);

            // act 
            var buyOrders = await _stockService.GetBuyOrders();

            // assert 
            Assert.NotNull(buyOrders);
            Assert.Equal(2, buyOrders.Count);

            Assert.Contains(buyOrders, order => order.StockSymbol == "MSFT");
            Assert.Contains(buyOrders, order => order.StockSymbol == "GOOG");
        }
        #endregion

        #region GetAllSellOrders
        // 1. When you invoke this method, by default, the returned list should be empty.
        [Fact]
        public async Task GetAllSellOrders_DefaultList_IsEmpty()
        {
            // Arrange

            // Act
            List<SellOrderResponse> sellOrders = await _stockService.GetSellOrders();

            // Assert
            Assert.NotNull(sellOrders);
            Assert.Empty(sellOrders);
        }

        // 2. When you first add few sell orders... the returned list should contain all... orders.
        [Fact]
        public async Task GetAllSellOrders_WithAddedOrders_ReturnsAllOrders()
        {
            // Arrange
            // Create a couple of valid requests
            SellOrderRequest request1 = CreateValidSellOrderRequest();
            request1.StockSymbol = "AAPL";

            SellOrderRequest request2 = CreateValidSellOrderRequest();
            request2.StockSymbol = "TSLA";

            // Add them using the service
            await _stockService.CreateSellOrder(request1);
            await _stockService.CreateSellOrder(request2);

            // Act
            List<SellOrderResponse> sellOrders = await _stockService.GetSellOrders();

            // Assert
            Assert.NotNull(sellOrders);
            Assert.Equal(2, sellOrders.Count);

            Assert.Contains(sellOrders, order => order.StockSymbol == "AAPL");
            Assert.Contains(sellOrders, order => order.StockSymbol == "TSLA");
        }
        #endregion
    }
}
