using OrderService.Domain.Entities;
using OrderService.Domain.Enums;

namespace OrderService.Application.DTOs
{
    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public string CustomerId { get; set; }
        public DateTime Timestamp { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderItem> Items { get; set; }
    }
}
