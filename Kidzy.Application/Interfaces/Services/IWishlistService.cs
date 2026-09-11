using Kidzy.Application.DTOs.Wishlist;

namespace Kidzy.Application.Interfaces.Services
{
    public interface IWishlistService
    {
        Task<WishlistResponseDto?> GetWishlistAsync(
            int userId);

        Task<WishlistResponseDto?> AddToWishlistAsync(
            int userId,
            AddToWishlistDto dto);

        Task<bool> RemoveFromWishlistAsync(
            int userId,
            int productId);

        Task<bool> ClearWishlistAsync(
            int userId);
    }
}