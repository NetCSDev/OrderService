using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Application.Features.Orders.Queries.GetOrderByIdQuery;
using OrderService.Core.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;

namespace OrderService.Tests.UnitTests.Application.Handlers
{
    public class GetOrderByIdHandlerTests
    {
        private readonly Mock<IOrderRepository> _mockRepo;
        private readonly Mock<ILogger<GetOrderByIdHandler>> _mockLogger;
        private readonly GetOrderByIdHandler _handler;

        public GetOrderByIdHandlerTests()
        {
            _mockRepo = new Mock<IOrderRepository>();
            _mockLogger = new Mock<ILogger<GetOrderByIdHandler>>();
            _handler = new GetOrderByIdHandler(_mockRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ReturnsOrderDto_WhenOrderExists()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                OrderId = orderId,
                CustomerId = "12345",
                Timestamp = DateTime.UtcNow,
                Status = OrderStatus.Confirmed,
                Items = new List<OrderItem>
            {
                new OrderItem { ProductId = Guid.NewGuid(), Quantity = 2 }
            }
            };

            _mockRepo.Setup(r => r.GetOrderByIdAsync(orderId)).ReturnsAsync(order);

            var query = new GetOrderByIdQuery(orderId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.OrderId);
            Assert.Equal(order.CustomerId, result.CustomerId);
        }

        [Fact]
        public async Task Handle_ReturnsNull_WhenOrderNotFound()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _mockRepo.Setup(r => r.GetOrderByIdAsync(orderId)).ReturnsAsync((Order)null);

            var query = new GetOrderByIdQuery(orderId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Handle_LogsErrorAndThrows_WhenExceptionOccurs()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var exception = new Exception("Database failure");

            _mockRepo.Setup(r => r.GetOrderByIdAsync(orderId)).ThrowsAsync(exception);

            var query = new GetOrderByIdQuery(orderId);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
            Assert.Equal("Database failure", ex.Message);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error retrieving order with ID")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
