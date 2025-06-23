using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Application.Features.Orders.Commands.CreateOrderCommand;
using OrderService.Core.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;

namespace OrderService.Tests.UnitTests.Application.Handlers
{
    public class CreateOrderHandlerTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<IKafkaProducer> _kafkaProducerMock;
        private readonly Mock<ILogger<CreateOrderHandler>> _loggerMock;
        private readonly CreateOrderHandler _createOrderHandler;

        public CreateOrderHandlerTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _notificationServiceMock = new Mock<INotificationService>();
            _kafkaProducerMock = new Mock<IKafkaProducer>();
            _loggerMock = new Mock<ILogger<CreateOrderHandler>>();
            _createOrderHandler = new CreateOrderHandler(
                _orderRepositoryMock.Object,
                _notificationServiceMock.Object,
                _kafkaProducerMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldCreateOrder_AndInvokeDependencies()
        {
            // Arrange
            var createOrderCommand = new CreateOrderCommand
            {
                CustomerId = "12345",
                Items = new List<OrderItem> { new OrderItem { ProductId = Guid.NewGuid(), Quantity = 1 } }
            };

            // Act
            var orderId = await _createOrderHandler.Handle(createOrderCommand, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, orderId);

            _orderRepositoryMock.Verify(repository => repository.AddOrderAsync(It.Is<Order>(
                order => order.CustomerId == createOrderCommand.CustomerId &&
                         order.Items == createOrderCommand.Items &&
                         order.Status == OrderStatus.Confirmed
            )), Times.Once);

            _notificationServiceMock.Verify(notificationService => notificationService.NotifyAsync(It.Is<Order>(
                order => order.CustomerId == createOrderCommand.CustomerId
            )), Times.Once);

            _kafkaProducerMock.Verify(kafkaProducer => kafkaProducer.PublishAsync("orders.created", It.IsAny<Order>()), Times.Once);
        }
    }
}