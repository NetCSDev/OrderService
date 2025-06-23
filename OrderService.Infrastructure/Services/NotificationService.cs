using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderService.Core.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Option;
using Polly;
using Polly.Retry;
using System.Text;

namespace OrderService.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly ILogger<NotificationService> _logger;
        private readonly IOptions<NotificationServiceOptions> _options;
        public NotificationService(HttpClient httpClient, ILogger<NotificationService> logger, IOptions<NotificationServiceOptions> options)
        {
            _httpClient = httpClient;
            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
            _logger = logger;
            _options = options;
        }

        public async Task NotifyAsync(Order order)
        {
            try
            {
                var payload = JsonConvert.SerializeObject(order);
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                await _retryPolicy.ExecuteAsync(() =>
                    _httpClient.PostAsync(_options.Value.BaseUrl, content));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

        }
    }
}
