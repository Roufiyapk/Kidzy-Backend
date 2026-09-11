using Kidzy.Domain.Entities;

namespace Kidzy.Application.Interfaces.Repositories
{
    public interface IWishlistRepository
    {
        Task<Wishlist?> GetByUserIdAsync(int userId);

        Task<Wishlist?> CreateAsync(Wishlist wishlist);

        Task<WishlistItem?> GetItemAsync(
            int wishlistId,
            int productId);

        Task<WishlistItem> AddItemAsync(
            WishlistItem item);

        Task DeleteItemAsync(
            WishlistItem item);

        Task ClearAsync(
            Wishlist wishlist);
    }
}