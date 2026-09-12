using Kidzy.Application.DTOs.Checkout;
using Kidzy.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kidzy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(
            ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(
            CheckoutDto dto)
        {
            var userIdClaim =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (!int.TryParse(
                userIdClaim,
                out var userId))
            {
                return Unauthorized();
            }

            var order =
                await _checkoutService.CheckoutAsync(
                    userId,
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
    }
}