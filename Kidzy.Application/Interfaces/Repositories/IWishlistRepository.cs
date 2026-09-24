using Kidzy.Domain.Entities;

namespace Kidzy.Application.Interfaces.Repositories;

public interface IWishlistRepository
{
    Task<Wishlist?> GetWishlistAsync(int userId);

    Task<Wishlist> CreateWishlistAsync(int userId);

    Task<Product?> GetProductAsync(int productId);

    Task<WishlistItem?> GetWishlistItemAsync(
        int userId,
        int productId);

    Task AddItemAsync(
        WishlistItem item);

    void RemoveItem(
        WishlistItem item);

    Task SaveChangesAsync();
}