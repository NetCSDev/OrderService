using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Core.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;

namespace OrderService.Application.Features.Orders.Commands.CreateOrderCommand
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _repo;
        private readonly INotificationService _notificationService;
        private readonly IKafkaProducer _kafka;
        private readonly ILogger<CreateOrderHandler> _logger;

        public CreateOrderHandler(IOrderRepository repo, INotificationService notificationService, IKafkaProducer kafka, ILogger<CreateOrderHandler> logger)
        {
            _repo = repo;
            _notificationService = notificationService;
            _kafka = kafka;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var order = new Order
                {
                    OrderId = Guid.NewGuid(),
                    CustomerId = request.CustomerId,
                    Items = request.Items,
                    Status = OrderStatus.Confirmed,
                    Timestamp = DateTime.UtcNow,
                };

                await _repo.AddOrderAsync(order);

                await _notificationService.NotifyAsync(order); // includes Polly retry
                await _kafka.PublishAsync("orders.created", order);

                return order.OrderId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                throw;
            }
        }
    }
}
