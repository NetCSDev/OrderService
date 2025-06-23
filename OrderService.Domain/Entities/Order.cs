using OrderService.Domain.Enums;

namespace OrderService.Domain.Entities
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public string CustomerId { get; set; }
        public DateTime Timestamp { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderItem> Items { get; set; } = new();
    }
}
