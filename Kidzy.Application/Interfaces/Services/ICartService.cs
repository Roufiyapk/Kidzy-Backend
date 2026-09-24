using Kidzy.Application.DTOs.Cart;

namespace Kidzy.Application.Interfaces.Services;

public interface ICartService
{
    Task<CartResponseDto> GetCartAsync(
        int userId);

    Task<CartItemDto> AddToCartAsync(
        int userId,
        AddToCartDto dto);

    Task<CartItemDto?> UpdateQuantityAsync(
        int userId,
        int cartItemId,
        UpdateCartItemDto dto);

    Task<bool> RemoveFromCartAsync(
        int userId,
        int cartItemId);

    Task ClearCartAsync(
        int userId);
}