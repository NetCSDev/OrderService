using OrderService.Domain.Entities;

namespace OrderService.Core.Interfaces
{
    public interface IOrderRepository
    {
        Task AddOrderAsync(Order order);
        Task<Order> GetOrderByIdAsync(Guid id);
    }
}
