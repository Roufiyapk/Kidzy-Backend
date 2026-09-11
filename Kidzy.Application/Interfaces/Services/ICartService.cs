using Kidzy.Application.DTOs.Cart;

namespace Kidzy.Application.Interfaces.Services
{
    public interface ICartService
    {
        Task<CartResponseDto?> GetCartAsync(int userId);

        Task<CartResponseDto?> AddToCartAsync(
            int userId,
            AddToCartDto dto);

        Task<CartResponseDto?> UpdateCartItemAsync(
            int userId,
            int itemId,
            UpdateCartItemDto dto);

        Task<bool> DeleteCartItemAsync(
            int userId,
            int itemId);

        Task<bool> ClearCartAsync(int userId);
    }
}