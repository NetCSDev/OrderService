using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderService.Core.Interfaces;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Kafka
{
    public class KafkaProducer : IKafkaProducer
    {
        private readonly IProducer<Null, string> _producer;
        private readonly ILogger<KafkaProducer> _logger;
        public KafkaProducer(IProducer<Null, string> producer, ILogger<KafkaProducer> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task PublishAsync(string topic, Order order)
        {
            try
            {
                var message = new Message<Null, string> { Value = JsonConvert.SerializeObject(new { order.OrderId, order.Timestamp }) };
                await _producer.ProduceAsync(topic, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
