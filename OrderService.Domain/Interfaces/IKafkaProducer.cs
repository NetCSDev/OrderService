using OrderService.Domain.Entities;

namespace OrderService.Core.Interfaces
{
    public interface IKafkaProducer
    {
        Task PublishAsync(string topic, Order order);
    }
}
