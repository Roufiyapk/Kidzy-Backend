using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Domain.Entities;
using Kidzy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kidzy.Infrastructure.Repositories;

public class OrderRepository
    : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }


    // GET CART

    public async Task<Cart?>
        GetCartAsync(
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


    // GET PRODUCT + VARIANTS

    public async Task<Product?>
        GetProductWithVariantsAsync(
            int productId)
    {
        return await _context.Products
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(
                p => p.Id == productId);
    }


    // USER ORDERS

    public async Task<List<Order>>
        GetUserOrdersAsync(
            int userId)
    {
        return await _context.Orders

            .Include(o => o.Items)
                .ThenInclude(i => i.Product)

            .Include(o => o.Items)
                .ThenInclude(i => i.ProductVariant)

            .Where(o => o.UserId == userId)

            .OrderByDescending(
                o => o.CreatedAt)

            .AsNoTracking()

            .ToListAsync();
    }


    
    // SINGLE ORDER

    public async Task<Order?>
        GetUserOrderByIdAsync(
            int userId,
            int orderId)
    {
        return await _context.Orders

            .Include(o => o.Items)
                .ThenInclude(i => i.Product)

            .Include(o => o.Items)
                .ThenInclude(i => i.ProductVariant)

            .FirstOrDefaultAsync(
                o =>
                    o.UserId == userId &&
                    o.Id == orderId);
    }


    // PAYMENT DUPLICATE CHECK

    public async Task<bool>
        IsPaymentAlreadyUsedAsync(
            string paymentId)
    {
        return await _context.Orders
            .AnyAsync(
                o =>
                    o.RazorpayPaymentId ==
                    paymentId);
    }


    // ADD ORDER

    public async Task AddAsync(
        Order order)
    {
        await _context.Orders
            .AddAsync(order);
    }


    // CLEAR CART

    public void RemoveCartItems(
        IEnumerable<CartItem> items)
    {
        _context.CartItems
            .RemoveRange(items);
    }


    // SAVE

    public async Task SaveChangesAsync()
    {
        await _context
            .SaveChangesAsync();
    }
}