using Kidzy.Domain.Entities;

namespace Kidzy.Application.Interfaces.Repositories;

public interface ICartRepository
{
    Task<Cart?> GetCartAsync(int userId);

    Task<Cart> CreateCartAsync(int userId);

    Task<CartItem?> GetCartItemAsync(
        int userId,
        int cartItemId);

    Task<CartItem?> GetExistingCartItemAsync(
        int userId,
        int productId,
        int? productVariantId);

    Task<Product?> GetProductWithVariantsAsync(
        int productId);

    Task AddCartItemAsync(CartItem cartItem);

    void UpdateCartItem(CartItem cartItem);

    void RemoveCartItem(CartItem cartItem);

    void RemoveCartItems(IEnumerable<CartItem> cartItems);

    Task SaveChangesAsync();
}