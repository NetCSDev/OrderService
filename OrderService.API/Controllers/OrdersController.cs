using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Features.Orders.Commands.CreateOrderCommand;
using OrderService.Application.Features.Orders.Queries.GetOrderByIdQuery;

namespace OrderService.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderCommand cmd, IMediator _mediator)
        {
            try
            {
                var id = await _mediator.Send(cmd);
                return CreatedAtAction(nameof(GetOrderById), new { id }, new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating order");
                return StatusCode(500, "An error occurred while creating the order.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            try
            {
                var result = await _mediator.Send(new GetOrderByIdQuery(id));
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving order with ID {OrderId}", id);
                return StatusCode(500, "An error occurred while retrieving the order.");
            }
        }
    }
}
