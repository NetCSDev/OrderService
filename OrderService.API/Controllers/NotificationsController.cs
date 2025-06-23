using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.Entities;
using System.Diagnostics;

namespace OrderService.API.Controllers
{
    [Route("notifications")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        [HttpPost]
        public IActionResult ReceiveNotification([FromBody] Order payload)
        {
            Debug.WriteLine("Notification received: " + payload.OrderId);
            return Ok(new { Message = "Notification received successfully." });
        }
    }
}
