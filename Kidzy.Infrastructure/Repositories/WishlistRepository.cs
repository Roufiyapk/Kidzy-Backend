using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Domain.Entities;
using Kidzy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kidzy.Infrastructure.Repositories
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly KidzyDbContext _context;

        public WishlistRepository(
            KidzyDbContext context)
        {
            _context = context;
        }

        public async Task<Wishlist?> GetByUserIdAsync(
            int userId)
        {
            return await _context.Wishlists
                .Include(w => w.WishlistItems)
                    .ThenInclude(wi => wi.Product)
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task<Wishlist?> CreateAsync(
            Wishlist wishlist)
        {
            _context.Wishlists.Add(wishlist);

            await _context.SaveChangesAsync();

            return wishlist;
        }

        public async Task<WishlistItem?> GetItemAsync(
            int wishlistId,
            int productId)
        {
            return await _context.WishlistItems
                .FirstOrDefaultAsync(wi =>
                    wi.WishlistId == wishlistId &&
                    wi.ProductId == productId);
        }

        public async Task<WishlistItem> AddItemAsync(
            WishlistItem item)
        {
            _context.WishlistItems.Add(item);

            await _context.SaveChangesAsync();

            return item;
        }

        public async Task DeleteItemAsync(
            WishlistItem item)
        {
            _context.WishlistItems.Remove(item);

            await _context.SaveChangesAsync();
        }

        public async Task ClearAsync(
            Wishlist wishlist)
        {
            _context.WishlistItems.RemoveRange(
                wishlist.WishlistItems);

            await _context.SaveChangesAsync();
        }
    }
}