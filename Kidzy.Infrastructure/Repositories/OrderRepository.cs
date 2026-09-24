using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Domain.Entities;
using Kidzy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kidzy.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    // Get logged-in user's cart

    public async Task<Cart?> GetCartAsync(
        int userId)
    {
        return await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .Include(c => c.Items)
                .ThenInclude(i => i.ProductVariant)
            .FirstOrDefaultAsync(
                c => c.UserId == userId);
    }


    // Get product and variants

    public async Task<Product?>
        GetProductWithVariantsAsync(
            int productId)
    {
        return await _context.Products
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(
                p => p.Id == productId);
    }


    // Get all orders of current user

    public async Task<List<Order>>
        GetUserOrdersAsync(int userId)
    {
        return await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.ProductVariant)
            .Where(o => o.UserId == userId)
            .OrderByDescending(
                o => o.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }


    // Get one order of current user

    public async Task<Order?>
        GetUserOrderByIdAsync(
            int userId,
            int orderId)
    {
        return await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.ProductVariant)
            .FirstOrDefaultAsync(
                o =>
                    o.UserId == userId &&
                    o.Id == orderId);
    }


    // Add order

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }


    // Remove cart items

    public void RemoveCartItems(
        IEnumerable<CartItem> items)
    {
        _context.CartItems.RemoveRange(items);
    }


    // Save

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}