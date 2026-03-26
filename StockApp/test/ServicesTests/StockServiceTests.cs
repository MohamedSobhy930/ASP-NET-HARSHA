using AutoFixture;
using Azure.Core;
using Entities;
using FluentAssertions;
using IServicesContracts;
using Microsoft.EntityFrameworkCore;
using Moq;
using ReposContracts;
using Services;
using Microsoft.Extensions.Logging;
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
        // mock the repo 
        private readonly Mock<IStocksRepo> _stockRepositoryMock;
        private readonly Mock<ILogger<StockService>> _loggerMock;
        private readonly IFixture _fixture;
        public StockServiceTests() 
        {
            _fixture = new Fixture();
            _stockRepositoryMock = new Mock<IStocksRepo>();
            _loggerMock = new Mock<ILogger<StockService>>();
            _stockService = new StockService(_loggerMock.Object, _stockRepositoryMock.Object);
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
            Func<Task> result = async ()=> await _stockService.CreateBuyOrder(request);

            // assert
            await result.Should().ThrowAsync<ArgumentNullException>();
        }
        //2. When you supply buyOrderQuantity as 0 (as per the specification, minimum is 1), it should throw ArgumentException.
        [Fact]
        public async Task CreateBuyOrder_QuantityisZero_ThrowsArgumentException()
        {
            // arrange 
            BuyOrderRequest request = _fixture.Create<BuyOrderRequest>();
            request.Quantity = 0;

            BuyOrder buyOrder = request.ToBuyOrder();
            _stockRepositoryMock.Setup(temp => temp.CreateBuyOrder(It.IsAny<BuyOrder>()))
                .ReturnsAsync(buyOrder);
            // act 
            Func<Task> result = async ()=>await _stockService.CreateBuyOrder(request);

            // assert 
            await result.Should().ThrowAsync<ArgumentException>();

        }
        //3. When you supply buyOrderQuantity as 100001 (as per the specification, maximum is 100000), it should throw ArgumentException.
        [Fact]
        public async Task CreateBuyOrder_QuantityIsBiggerThan100000_ThrowsArgumentException()
        {
            // arrange 
            BuyOrderRequest request = _fixture.Create<BuyOrderRequest>();
            request.Quantity = 100001;

            BuyOrder buyOrder = request.ToBuyOrder();
            _stockRepositoryMock.Setup(temp => temp.CreateBuyOrder(It.IsAny<BuyOrder>()))
                .ReturnsAsync(buyOrder);
            // act 
            Func<Task> result = async () => await _stockService.CreateBuyOrder(request);

            // assert 
            await result.Should().ThrowAsync<ArgumentException>();
        }
        //4. When you supply buyOrderPrice as 0 (as per the specification, minimum is 1), it should throw ArgumentException.
        [Fact]
        public async Task CreateBuyOrder_PriceIsZero_ThrowsArgumentException()
        {
            // arrange 
            BuyOrderRequest request = _fixture.Build<BuyOrderRequest>()
                .With(temp => temp.Price , 0)
                .Create();

            BuyOrder buyOrder = request.ToBuyOrder();
            _stockRepositoryMock.Setup(temp => temp.CreateBuyOrder(It.IsAny<BuyOrder>()))
                .ReturnsAsync(buyOrder);
            // act 
            Func<Task> result = async () => await _stockService.CreateBuyOrder(request);

            // assert 
            await result.Should().ThrowAsync<ArgumentException>();
        }
        //5. When you supply buyOrderPrice as 10001 (as per the specification, maximum is 10000), it should throw ArgumentException.
        [Fact]
        public async Task CreateBuyOrder_PriceIsTooHigh_ThrowsArgumentException()
        {
            // arrange 
            BuyOrderRequest request = _fixture.Build<BuyOrderRequest>()
                .With(temp => temp.Price, 10001)
                .Create();
            BuyOrder buyOrder = request.ToBuyOrder();
            _stockRepositoryMock.Setup(temp => temp.CreateBuyOrder(It.IsAny<BuyOrder>()))
                .ReturnsAsync(buyOrder);
            Func<Task> result = async () => await _stockService.CreateBuyOrder(request);

            // assert 
            await result.Should().ThrowAsync<ArgumentException>();
        }
        //6. When you supply stock symbol=null (as per the specification, stock symbol can't be null), it should throw ArgumentException.
        [Fact]
        public async Task CreateBuyOrder_SymbolIsNull_ThrowsArgumentException()
        {
            // arrange 
            BuyOrderRequest buyOrderRequest = _fixture.Build<BuyOrderRequest>()
                .With(temp => temp.StockSymbol, null as string)
                .Create();

            BuyOrder buyOrder = buyOrderRequest.ToBuyOrder();
            _stockRepositoryMock.Setup(temp => temp.CreateBuyOrder(It.IsAny<BuyOrder>()))
                .ReturnsAsync(buyOrder);
            Func<Task> result = async () => await _stockService.CreateBuyOrder(buyOrderRequest);

            // assert 
            await result.Should().ThrowAsync<ArgumentException>();
        }
        //7. When you supply dateAndTimeOfOrder as "1999-12-31" (YYYY-MM-DD) - (as per the specification, it should be equal or newer date than 2000-01-01), it should throw ArgumentException.
        [Fact]
        public async Task CreateBuyOrder_DateAndTimeOfOrderisWrong_ThrowsArgumentException()
        {
            // arrange 
            BuyOrderRequest buyOrderRequest = _fixture.Build<BuyOrderRequest>()
                .With(temp => temp.DateAndTimeOfOrder , DateTime.Parse("1999-12-31"))
                .Create();

            BuyOrder buyOrder = buyOrderRequest.ToBuyOrder();
            _stockRepositoryMock.Setup(temp => temp.CreateBuyOrder(It.IsAny<BuyOrder>()))
                .ReturnsAsync(buyOrder);

            // act
            Func<Task> result = async () => await _stockService.CreateBuyOrder(buyOrderRequest);

            // assert 
            await result.Should().ThrowAsync<ArgumentException>();
        }
        //8. If you supply all valid values, it should be successful and return an object of BuyOrderResponse type with auto-generated BuyOrderID(guid).
        [Fact]
        public async Task CreateBuyOrder_ProperOrder_ReturnBuyOrderResponse()
        {
            // arrange
            BuyOrderRequest request = _fixture.Create<BuyOrderRequest>();

            BuyOrder buyOrder = request.ToBuyOrder();
            _stockRepositoryMock.Setup(temp => temp.CreateBuyOrder(It.IsAny<BuyOrder>()))
                .ReturnsAsync(buyOrder);
            // act
            BuyOrderResponse response = await _stockService.CreateBuyOrder(request);

            // assert
            response.Should().NotBeNull();
            response.Should().BeOfType<BuyOrderResponse>();
            response.BuyOrderID.Should().Be(buyOrder.BuyOrderID);
            response.TradeAmount.Should().Be(request.Quantity * request.Price);
            
        }
        #endregion

        #region CreateSellOrder
        // 1. When you supply SellOrderRequest as null, it should throw ArgumentNullException.
        [Fact]
        public async Task CreateSellOrder_NullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            SellOrderRequest? request = null;

            // Act 
            Func<Task> result = async () => await _stockService.CreateSellOrder(request);

            // Assert
            await result.Should().ThrowAsync<ArgumentNullException>();
        }
        // 2. When you supply sellOrderQuantity as 0, it should throw ArgumentException.
        [Fact]
        public async Task CreateSellOrder_QuantityIsZero_ThrowsArgumentException()
        {
            // Arrange
            SellOrderRequest request = _fixture.Create<SellOrderRequest>();
            request.Quantity = 0;

            SellOrder sellOrder = request.ToSellOrder();
            _stockRepositoryMock.Setup(temp => temp.CreateSellOrder(It.IsAny<SellOrder>()))
                .ReturnsAsync(sellOrder);
            // Act 
            Func<Task> result = async () => await _stockService.CreateSellOrder(request);

            // Assert
            await result.Should().ThrowAsync<ArgumentException>();
        }
        // 3. When you supply sellOrderQuantity as 100001, it should throw ArgumentException.
        [Fact]
        public async Task CreateSellOrder_QuantityTooHigh_ThrowsArgumentException()
        {
            // Arrange
            SellOrderRequest request = _fixture.Create<SellOrderRequest>();
            request.Quantity = 100001;

            SellOrder sellOrder = request.ToSellOrder();
            _stockRepositoryMock.Setup(temp => temp.CreateSellOrder(It.IsAny<SellOrder>()))
                .ReturnsAsync(sellOrder);
            // Act 
            Func<Task> result = async () => await _stockService.CreateSellOrder(request);

            // Assert
            await result.Should().ThrowAsync<ArgumentException>();
        }
        // 4. When you supply sellOrderPrice as 0, it should throw ArgumentException.
        [Fact]
        public async Task CreateSellOrder_PriceIsZero_ThrowsArgumentException()
        {
            // Arrange
            SellOrderRequest request = _fixture.Build<SellOrderRequest>()
                .With(temp => temp.Price, 0)
                .Create();

            SellOrder sellOrder = request.ToSellOrder();
            _stockRepositoryMock.Setup(temp => temp.CreateSellOrder(It.IsAny<SellOrder>()))
                .ReturnsAsync(sellOrder);
            // Act 
            Func<Task> result = async () => await _stockService.CreateSellOrder(request);

            // Assert
            await result.Should().ThrowAsync<ArgumentException>();
        }
        // 5. When you supply sellOrderPrice as 10001, it should throw ArgumentException.
        [Fact]
        public async Task CreateSellOrder_PriceTooHigh_ThrowsArgumentException()
        {
            // Arrange
            SellOrderRequest request = _fixture.Build<SellOrderRequest>()
                .With(temp => temp.Price, 10001)
                .Create();

            SellOrder sellOrder = request.ToSellOrder();
            _stockRepositoryMock.Setup(temp => temp.CreateSellOrder(It.IsAny<SellOrder>()))
                .ReturnsAsync(sellOrder);
            // Act 
            Func<Task> result = async () => await _stockService.CreateSellOrder(request);

            // Assert
            await result.Should().ThrowAsync<ArgumentException>();
        }
        // 6. When you supply stock symbol=null, it should throw ArgumentException.
        [Fact]
        public async Task CreateSellOrder_NullStockSymbol_ThrowsArgumentException()
        {
            // Arrange
            SellOrderRequest request = _fixture.Build<SellOrderRequest>()
                .With(temp => temp.StockSymbol, null as string)
                .Create();

            SellOrder sellOrder = request.ToSellOrder();
            _stockRepositoryMock.Setup(temp => temp.CreateSellOrder(It.IsAny<SellOrder>()))
                .ReturnsAsync(sellOrder);
            // Act 
            Func<Task> result = async () => await _stockService.CreateSellOrder(request);

            // Assert
            await result.Should().ThrowAsync<ArgumentException>();
        }
        // 7. When you supply dateAndTimeOfOrder as "1999-12-31", it should throw ArgumentException.
        [Fact]
        public async Task CreateSellOrder_DateTooOld_ThrowsArgumentException()
        {
            // Arrange
            SellOrderRequest request = _fixture.Build<SellOrderRequest>()
                .With(temp => temp.DateAndTimeOfOrder, DateTime.Parse("1999-12-31"))
                .Create();

            SellOrder sellOrder = request.ToSellOrder();
            _stockRepositoryMock.Setup(temp => temp.CreateSellOrder(It.IsAny<SellOrder>()))
                .ReturnsAsync(sellOrder);
            // Act
            Func<Task> result = async () => await _stockService.CreateSellOrder(request);

            // Assert
            await result.Should().ThrowAsync<ArgumentException>();
        }
        // 8. If you supply all valid values, it should be successful and return SellOrderResponse.
        [Fact]
        public async Task CreateSellOrder_ValidRequest_ReturnsSellOrderResponse()
        {
            // Arrange
            SellOrderRequest request = _fixture.Create<SellOrderRequest>();

            SellOrder sellOrder = request.ToSellOrder();
            _stockRepositoryMock.Setup(temp => temp.CreateSellOrder(It.IsAny<SellOrder>()))
                .ReturnsAsync(sellOrder);
            // Act
            SellOrderResponse response = await _stockService.CreateSellOrder(request);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<SellOrderResponse>();
            response.SellOrderID.Should().Be(sellOrder.SellOrderID);
            response.TradeAmount.Should().Be(request.Quantity * request.Price);
        
        }
        #endregion

        #region GetAllBuyOrders
        [Fact]
        public async Task GetAllBuyOrders_DefaultList_ReturnEmpty()
        {
            // arrange
            List<BuyOrder> buyOrders = new List<BuyOrder>();
            _stockRepositoryMock.Setup(temp => temp.GetBuyOrders())
                .ReturnsAsync(buyOrders);
            // act
            var response =await _stockService.GetBuyOrders();

            // assert
            response.Should().BeEmpty();

        }
        [Fact]
        public async Task GetAllBuyOrders_OrdersFound_ReturnList()
        {
            // arrange
            List<BuyOrder> buyOrders = new List<BuyOrder>()
            {
                _fixture.Build<BuyOrder>()
                .With(temp => temp.StockSymbol, "MSFT")
                .Create(),
                _fixture.Build<BuyOrder>()
                .With(temp => temp.StockSymbol, "GOOG")
                .Create()
            };
            List<BuyOrderResponse> expectedResponses = new List<BuyOrderResponse>()
            {
                buyOrders[0].ToBuyOrderResponse(),
                buyOrders[1].ToBuyOrderResponse()
            };
            _stockRepositoryMock.Setup(temp => temp.GetBuyOrders())
                .ReturnsAsync(buyOrders);


            // act 
            var result = await _stockService.GetBuyOrders();

            // assert 
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expectedResponses);
        }
        #endregion

        #region GetAllSellOrders
        // 1. When you invoke this method, by default, the returned list should be empty.
        [Fact]
        public async Task GetAllSellOrders_DefaultList_IsEmpty()
        {
            // Arrange
            List<SellOrder> sellOrders = new List<SellOrder>();
            _stockRepositoryMock.Setup(temp => temp.GetSellOrders())
                .ReturnsAsync(sellOrders);
            // Act
            List<SellOrderResponse> result = await _stockService.GetSellOrders();

            // Assert
            result.Should().BeEmpty();  
        }

        // 2. When you first add few sell orders... the returned list should contain all... orders.
        [Fact]
        public async Task GetAllSellOrders_WithAddedOrders_ReturnsAllOrders()
        {
            // Arrange
            List<SellOrder> sellOrdersFromRepo = new List<SellOrder>()
            {
                _fixture.Build<SellOrder>()
                .With(temp => temp.StockSymbol, "AAPL")
                .Create(),
                _fixture.Build<SellOrder>()
                .With(temp => temp.StockSymbol, "TSLA")
                .Create()
            };
            List<SellOrderResponse> expectedResponses = new List<SellOrderResponse>()
            {
                sellOrdersFromRepo[0].ToSellOrderResponse(),
                sellOrdersFromRepo[1].ToSellOrderResponse()
            };
            _stockRepositoryMock.Setup(temp => temp.GetSellOrders())
                .ReturnsAsync(sellOrdersFromRepo);

            // Act
            List<SellOrderResponse> sellOrders = await _stockService.GetSellOrders();

            // Assert
            sellOrders.Should().HaveCount(2);
            sellOrders.Should().BeEquivalentTo(expectedResponses);
        }
        #endregion
    }
}
