using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Domain.Entities;
using Kidzy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kidzy.Infrastructure.Repositories;

public class WishlistRepository : IWishlistRepository
{
    private readonly ApplicationDbContext _context;

    public WishlistRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Wishlist?> GetWishlistAsync(
        int userId)
    {
        return await _context.Wishlists
            .Include(w => w.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(
                w => w.UserId == userId);
    }

    public async Task<Wishlist> CreateWishlistAsync(
        int userId)
    {
        var wishlist = new Wishlist
        {
            UserId = userId
        };

        await _context.Wishlists.AddAsync(
            wishlist);

        await SaveChangesAsync();

        return wishlist;
    }

    public async Task<Product?> GetProductAsync(
        int productId)
    {
        return await _context.Products
            .FirstOrDefaultAsync(
                p => p.Id == productId);
    }

    public async Task<WishlistItem?> GetWishlistItemAsync(
        int userId,
        int productId)
    {
        return await _context.WishlistItems
            .Include(i => i.Wishlist)
            .Include(i => i.Product)
            .FirstOrDefaultAsync(
                i =>
                    i.Wishlist.UserId == userId &&
                    i.ProductId == productId);
    }

    public async Task AddItemAsync(
        WishlistItem item)
    {
        await _context.WishlistItems.AddAsync(item);
    }

    public void RemoveItem(
        WishlistItem item)
    {
        _context.WishlistItems.Remove(item);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}