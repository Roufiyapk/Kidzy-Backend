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


        // CART CHECKOUT
        // POST: api/Checkout

        [HttpPost]
        public async Task<IActionResult> Checkout(
            CheckoutDto dto)
        {
            // Get logged-in user's ID from JWT
            var userIdClaim =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            // Invalid token/user ID
            if (!int.TryParse(
                    userIdClaim,
                    out var userId))
            {
                return Unauthorized();
            }

            // Checkout cart
            var order =
                await _checkoutService
                    .CheckoutAsync(userId, dto);

            // Empty cart / stock problem
            if (order == null)
            {
                return BadRequest(new
                {
                    message =
                        "Cart is empty or product is out of stock"
                });
            }

            return Ok(order);
        }


        // BUY NOW
        // POST: api/Checkout/buy-now

        [HttpPost("buy-now")]
        public async Task<IActionResult> BuyNow(
            BuyNowDto dto)
        {
            // Get logged-in user's ID from JWT
            var userIdClaim =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            // Invalid token/user ID
            if (!int.TryParse(
                    userIdClaim,
                    out var userId))
            {
                return Unauthorized();
            }

            // Buy Now checkout
            var order =
                await _checkoutService
                    .BuyNowAsync(userId, dto);

            // Product / size / stock problem
            if (order == null)
            {
                return BadRequest(new
                {
                    message =
                        "Product not found, size unavailable, or product is out of stock"
                });
            }

            return Ok(order);
        }
    }
}