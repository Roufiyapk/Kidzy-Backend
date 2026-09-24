using Kidzy.Application.DTOs.Wishlist;

namespace Kidzy.Application.Interfaces.Services;

public interface IWishlistService
{
    Task<WishlistResponseDto> GetWishlistAsync(
        int userId);

    Task<WishlistItemDto> AddToWishlistAsync(
        int userId,
        AddToWishlistDto dto);

    Task<bool> RemoveFromWishlistAsync(
        int userId,
        int productId);
}