using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs;
using OrderService.Core.Interfaces;

namespace OrderService.Application.Features.Orders.Queries.GetOrderByIdQuery
{

    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<GetOrderByIdHandler> _logger;

        public GetOrderByIdHandler(IOrderRepository orderRepository, ILogger<GetOrderByIdHandler> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {

                var order = await _orderRepository.GetOrderByIdAsync(request.OrderId);
                if (order == null) return null;

                return new OrderDto
                {
                    OrderId = order.OrderId,
                    CustomerId = order.CustomerId,
                    Timestamp = order.Timestamp,
                    Status = order.Status,
                    Items = order.Items
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order with ID {OrderId}", request.OrderId);
                throw;
            }
        }
    }
}
