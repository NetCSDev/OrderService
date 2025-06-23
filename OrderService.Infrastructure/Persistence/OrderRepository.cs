using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderService.Core.Interfaces;
using StackExchange.Redis;
using Order = OrderService.Domain.Entities.Order;

namespace OrderService.Infrastructure.Persistence
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly IDatabase _cache;
        private readonly ILogger<OrderRepository> _logger;
        public OrderRepository(AppDbContext context, IConnectionMultiplexer redis, ILogger<OrderRepository> logger)
        {
            _context = context;
            _cache = redis.GetDatabase();
            _logger = logger;
        }

        public async Task AddOrderAsync(Order order)
        {
            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<Order> GetOrderByIdAsync(Guid id)
        {
            var cached = await _cache.StringGetAsync(id.ToString());
            if (cached.HasValue)
                return JsonConvert.DeserializeObject<Order>(cached);

            var order = await _context.Orders.FindAsync(id);
            if (order != null)
                await _cache.StringSetAsync(id.ToString(), JsonConvert.SerializeObject(order), TimeSpan.FromMinutes(5));

            return order;
        }
    }
}
