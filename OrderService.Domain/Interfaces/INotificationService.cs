using OrderService.Domain.Entities;

namespace OrderService.Core.Interfaces
{
    public interface INotificationService
    {
        Task NotifyAsync(Order order);
    }
}
