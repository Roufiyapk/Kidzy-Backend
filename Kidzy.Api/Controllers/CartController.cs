using System.Security.Claims;
using Kidzy.Application.DTOs.Cart;
using Kidzy.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kidzy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(
            ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: api/Cart
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var cart =
                await _cartService.GetCartAsync(
                    userId.Value);

            if (cart == null)
            {
                return Ok(new
                {
                    userId = userId.Value,
                    items = new List<object>(),
                    totalPrice = 0
                });
            }

            return Ok(cart);
        }

        // POST: api/Cart
        [HttpPost]
        public async Task<IActionResult> AddToCart(
            AddToCartDto dto)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var cart =
                await _cartService.AddToCartAsync(
                    userId.Value,
                    dto);

            if (cart == null)
            {
                return BadRequest(new
                {
                    message =
                        "Unable to add product to cart. Check product, size, quantity and stock."
                });
            }

            return Ok(cart);
        }

        // PUT: api/Cart/1
        [HttpPut("{itemId}")]
        public async Task<IActionResult> UpdateCartItem(
            int itemId,
            UpdateCartItemDto dto)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var cart =
                await _cartService.UpdateCartItemAsync(
                    userId.Value,
                    itemId,
                    dto);

            if (cart == null)
            {
                return BadRequest(new
                {
                    message =
                        "Unable to update cart item."
                });
            }

            return Ok(cart);
        }

        // DELETE: api/Cart/1
        [HttpDelete("{itemId}")]
        public async Task<IActionResult> DeleteCartItem(
            int itemId)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var deleted =
                await _cartService.DeleteCartItemAsync(
                    userId.Value,
                    itemId);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Cart item not found"
                });
            }

            return Ok(new
            {
                message =
                    "Cart item deleted successfully"
            });
        }

        // DELETE: api/Cart/clear
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var cleared =
                await _cartService.ClearCartAsync(
                    userId.Value);

            if (!cleared)
            {
                return NotFound(new
                {
                    message = "Cart not found"
                });
            }

            return Ok(new
            {
                message = "Cart cleared successfully"
            });
        }

        private int? GetUserId()
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (int.TryParse(userId, out var id))
            {
                return id;
            }

            return null;
        }
    }
}