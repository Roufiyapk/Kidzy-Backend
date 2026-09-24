using System.Net;
using System.Security.Claims;
using Kidzy.API.Contracts;
using Kidzy.Application.DTOs.Wishlist;
using Kidzy.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kidzy.API.Controllers.User;

[ApiController]
[Route("api/wishlist")]
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
        try
        {
            var userId = GetUserId();

            var wishlist =
                await _wishlistService
                    .GetWishlistAsync(userId);

            return Ok(
                ApiResponse<WishlistResponseDto>.Success(
                    wishlist,
                    "Wishlist retrieved successfully."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(
                ApiResponse<WishlistResponseDto>.Fail(
                    new List<string>
                    {
                        ex.Message
                    },
                    "Unauthorized.",
                    HttpStatusCode.Unauthorized));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<WishlistResponseDto>.Fail(
                    new List<string>
                    {
                        ex.Message
                    },
                    "Failed to retrieve wishlist."));
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddToWishlist(
        [FromBody] AddToWishlistDto dto)
    {
        try
        {
            var userId = GetUserId();

            var item =
                await _wishlistService
                    .AddToWishlistAsync(
                        userId,
                        dto);

            return Ok(
                ApiResponse<WishlistItemDto>.Success(
                    item,
                    "Product added to wishlist."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(
                ApiResponse<WishlistItemDto>.Fail(
                    new List<string>
                    {
                        ex.Message
                    },
                    "Unauthorized.",
                    HttpStatusCode.Unauthorized));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<WishlistItemDto>.Fail(
                    new List<string>
                    {
                        ex.Message
                    },
                    "Failed to add product to wishlist."));
        }
    }

    [HttpDelete("{productId:int}")]
    public async Task<IActionResult> RemoveFromWishlist(
        int productId)
    {
        try
        {
            var userId = GetUserId();

            var removed =
                await _wishlistService
                    .RemoveFromWishlistAsync(
                        userId,
                        productId);

            if (!removed)
            {
                return NotFound(
                    ApiResponse<string>.Fail(
                        new List<string>
                        {
                            "Product not found in wishlist."
                        },
                        "Wishlist item not found.",
                        HttpStatusCode.NotFound));
            }

            return Ok(
                ApiResponse<string>.Success(
                    string.Empty,
                    "Product removed from wishlist."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(
                ApiResponse<string>.Fail(
                    new List<string>
                    {
                        ex.Message
                    },
                    "Unauthorized.",
                    HttpStatusCode.Unauthorized));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<string>.Fail(
                    new List<string>
                    {
                        ex.Message
                    },
                    "Failed to remove product from wishlist."));
        }
    }

    private int GetUserId()
    {
        var userId =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedAccessException(
                "User ID not found in token.");
        }

        if (!int.TryParse(
                userId,
                out var parsedUserId))
        {
            throw new UnauthorizedAccessException(
                "Invalid user ID in token.");
        }

        return parsedUserId;
    }
}