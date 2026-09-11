using Kidzy.Application.DTOs.Wishlist;
using Kidzy.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kidzy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(
            IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }


        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var wishlist =
                await _wishlistService.GetWishlistAsync(
                    userId.Value);

            if (wishlist == null)
            {
                return NotFound(new
                {
                    message = "Wishlist not found"
                });
            }

            return Ok(wishlist);
        }


        [HttpPost]
        public async Task<IActionResult> AddToWishlist(
            AddToWishlistDto dto)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var wishlist =
                await _wishlistService.AddToWishlistAsync(
                    userId.Value,
                    dto);

            if (wishlist == null)
            {
                return NotFound(new
                {
                    message = "Product not found"
                });
            }

            return Ok(wishlist);
        }


        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(
            int productId)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var result =
                await _wishlistService.RemoveFromWishlistAsync(
                    userId.Value,
                    productId);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Wishlist item not found"
                });
            }

            return Ok(new
            {
                message = "Product removed from wishlist"
            });
        }


        [HttpDelete("clear")]
        public async Task<IActionResult> ClearWishlist()
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var result =
                await _wishlistService.ClearWishlistAsync(
                    userId.Value);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Wishlist not found"
                });
            }

            return Ok(new
            {
                message = "Wishlist cleared"
            });
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