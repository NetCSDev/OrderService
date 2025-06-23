using MediatR;
using OrderService.Domain.Entities;

namespace OrderService.Application.Features.Orders.Commands.CreateOrderCommand
{
    public class CreateOrderCommand : IRequest<Guid>
    {
        public string CustomerId { get; set; }
        public List<OrderItem> Items { get; set; }
    }
}
