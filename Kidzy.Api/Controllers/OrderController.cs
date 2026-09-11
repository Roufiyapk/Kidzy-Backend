using Kidzy.Application.DTOs.Order;
using Kidzy.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kidzy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(
            IOrderService orderService)
        {
            _orderService = orderService;
        }

        // POST: api/Order
        [HttpPost]
        public async Task<IActionResult> CreateOrder(
            CreateOrderDto dto)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var order =
                await _orderService.CreateOrderAsync(
                    userId.Value,
                    dto);

            if (order == null)
            {
                return BadRequest(new
                {
                    message = "Cart is empty"
                });
            }

            return Ok(order);
        }

        // GET: api/Order
        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var orders =
                await _orderService.GetMyOrdersAsync(
                    userId.Value);

            return Ok(orders);
        }

        // GET: api/Order/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(
            int id)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var order =
                await _orderService.GetOrderByIdAsync(
                    userId.Value,
                    id);

            if (order == null)
            {
                return NotFound(new
                {
                    message = "Order not found"
                });
            }

            return Ok(order);
        }

        private int? GetUserId()
        {
            var userIdClaim =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (int.TryParse(
                userIdClaim,
                out var userId))
            {
                return userId;
            }

            return null;
        }
    }
}