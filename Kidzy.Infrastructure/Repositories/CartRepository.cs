using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Domain.Entities;
using Kidzy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kidzy.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly KidzyDbContext _context;

        public CartRepository(KidzyDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetByUserIdAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart?> GetByIdAsync(int cartId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }

        public async Task<CartItem?> GetItemAsync(
            int cartId,
            int productId,
            string selectedSize)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(ci =>
                    ci.CartId == cartId &&
                    ci.ProductId == productId &&
                    ci.SelectedSize == selectedSize);
        }

        public async Task<CartItem?> GetItemByIdAsync(int itemId)
        {
            return await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == itemId);
        }

        public async Task<Cart> CreateAsync(Cart cart)
        {
            _context.Carts.Add(cart);

            await _context.SaveChangesAsync();

            return cart;
        }

        public async Task<CartItem> AddItemAsync(
            CartItem cartItem)
        {
            _context.CartItems.Add(cartItem);

            await _context.SaveChangesAsync();

            return cartItem;
        }

        public async Task UpdateItemAsync(
            CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(
            CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);

            await _context.SaveChangesAsync();
        }

        public async Task ClearAsync(Cart cart)
        {
            _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync();
        }
    }
}