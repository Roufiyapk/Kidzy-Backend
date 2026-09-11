using Kidzy.Domain.Entities;

namespace Kidzy.Application.Interfaces.Repositories
{
    public interface ICartRepository
    {
        Task<Cart?> GetByUserIdAsync(int userId);

        Task<Cart?> GetByIdAsync(int cartId);

        Task<CartItem?> GetItemAsync(
            int cartId,
            int productId,
            string selectedSize);

        Task<CartItem?> GetItemByIdAsync(int itemId);

        Task<Cart> CreateAsync(Cart cart);

        Task<CartItem> AddItemAsync(CartItem cartItem);

        Task UpdateItemAsync(CartItem cartItem);

        Task DeleteItemAsync(CartItem cartItem);

        Task ClearAsync(Cart cart);
    }
}