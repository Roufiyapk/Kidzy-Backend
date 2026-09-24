using System.Net;
using System.Security.Claims;
using Kidzy.API.Contracts;
using Kidzy.Application.DTOs.Cart;
using Kidzy.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kidzy.API.Controllers.User;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    // GET CART

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        try
        {
            var userId = GetUserId();

            var cart =
                await _cartService.GetCartAsync(userId);

            return Ok(
                ApiResponse<CartResponseDto>.Success(
                    cart,
                    "Cart retrieved successfully."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(
                ApiResponse<CartResponseDto>.Fail(
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
                ApiResponse<CartResponseDto>.Fail(
                    new List<string>
                    {
                        ex.Message
                    },
                    "Failed to retrieve cart."));
        }
    }


    // ADD TO CART

    [HttpPost]
    public async Task<IActionResult> AddToCart(
        [FromBody] AddToCartDto dto)
    {
        try
        {
            var userId = GetUserId();

            var item =
                await _cartService.AddToCartAsync(
                    userId,
                    dto);

            return Ok(
                ApiResponse<CartItemDto>.Success(
                    item,
                    "Product added to cart."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(
                ApiResponse<CartItemDto>.Fail(
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
                ApiResponse<CartItemDto>.Fail(
                    new List<string>
                    {
                        ex.Message
                    },
                    "Failed to add product to cart."));
        }
    }


    // UPDATE QUANTITY

    [HttpPut("{cartItemId:int}")]
    public async Task<IActionResult> UpdateQuantity(
        int cartItemId,
        [FromBody] UpdateCartItemDto dto)
    {
        try
        {
            var userId = GetUserId();

            var item =
                await _cartService.UpdateQuantityAsync(
                    userId,
                    cartItemId,
                    dto);

            if (item == null)
            {
                return NotFound(
                    ApiResponse<CartItemDto>.Fail(
                        new List<string>
                        {
                            "Cart item not found."
                        },
                        "Cart item not found.",
                        HttpStatusCode.NotFound));
            }

            return Ok(
                ApiResponse<CartItemDto>.Success(
                    item,
                    "Cart quantity updated successfully."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(
                ApiResponse<CartItemDto>.Fail(
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
                ApiResponse<CartItemDto>.Fail(
                    new List<string>
                    {
                        ex.Message
                    },
                    "Failed to update cart quantity."));
        }
    }


    // REMOVE ITEM

    [HttpDelete("{cartItemId:int}")]
    public async Task<IActionResult> RemoveFromCart(
        int cartItemId)
    {
        try
        {
            var userId = GetUserId();

            var deleted =
                await _cartService.RemoveFromCartAsync(
                    userId,
                    cartItemId);

            if (!deleted)
            {
                return NotFound(
                    ApiResponse<string>.Fail(
                        new List<string>
                        {
                            "Cart item not found."
                        },
                        "Cart item not found.",
                        HttpStatusCode.NotFound));
            }

            return Ok(
                ApiResponse<string>.Success(
                    string.Empty,
                    "Item removed from cart."));
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
                    "Failed to remove item from cart."));
        }
    }


    // CLEAR CART

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        try
        {
            var userId = GetUserId();

            await _cartService.ClearCartAsync(userId);

            return Ok(
                ApiResponse<string>.Success(
                    string.Empty,
                    "Cart cleared successfully."));
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
                    "Failed to clear cart."));
        }
    }


    // GET USER ID FROM JWT

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