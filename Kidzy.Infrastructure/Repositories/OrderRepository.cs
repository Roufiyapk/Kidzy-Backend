using Kidzy.Application.Interfaces.Repositories;

using Kidzy.Domain.Entities;
using Kidzy.Domain.Enums;

using Kidzy.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace Kidzy.Infrastructure.Repositories;

public class OrderRepository
    : IOrderRepository
{
    private readonly ApplicationDbContext
        _context;


    public OrderRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }


    // CART

    public async Task<Cart?>
        GetCartAsync(
            int userId)
    {
        return await _context.Carts

            .Include(
                x => x.Items)

                .ThenInclude(
                    x => x.Product)

            .Include(
                x => x.Items)

                .ThenInclude(
                    x => x.ProductVariant)

            .FirstOrDefaultAsync(
                x =>
                    x.UserId ==
                    userId);
    }


    // PRODUCT + VARIANTS

    public async Task<Product?>
        GetProductWithVariantsAsync(
            int productId)
    {
        return await _context.Products

            .Include(
                x => x.Variants)

            .FirstOrDefaultAsync(
                x =>
                    x.Id ==
                    productId);
    }


    // USER ORDERS

    public async Task<List<Order>>
        GetUserOrdersAsync(
            int userId)
    {
        return await _context.Orders

            .Include(
                x => x.User)

            .Include(
                x => x.Payment)

            .Include(
                x => x.Items)

                .ThenInclude(
                    x => x.Product)

            .Include(
                x => x.Items)

                .ThenInclude(
                    x => x.ProductVariant)

            .Where(
                x =>
                    x.UserId ==
                    userId)

            .OrderByDescending(
                x =>
                    x.CreatedAt)

            .ToListAsync();
    }


    // ONE ORDER

    public async Task<Order?>
        GetOrderByIdAsync(
            int orderId)
    {
        return await _context.Orders

            .Include(
                x => x.User)

            .Include(
                x => x.Payment)

            .Include(
                x => x.Items)

                .ThenInclude(
                    x => x.Product)

            .Include(
                x => x.Items)

                .ThenInclude(
                    x => x.ProductVariant)

            .FirstOrDefaultAsync(
                x =>
                    x.Id ==
                    orderId);
    }


    // ALL ORDERS

    public async Task<List<Order>>
        GetAllOrdersAsync(
            OrderStatus? status = null)
    {
        IQueryable<Order> query =
            _context.Orders

                .Include(
                    x => x.User)

                .Include(
                    x => x.Payment)

                .Include(
                    x => x.Items)

                    .ThenInclude(
                        x => x.Product)

                .Include(
                    x => x.Items)

                    .ThenInclude(
                        x => x.ProductVariant);


        if (status.HasValue)
        {
            query =
                query.Where(
                    x =>
                        x.Status ==
                        status.Value);
        }


        return await query

            .OrderByDescending(
                x =>
                    x.CreatedAt)

            .ToListAsync();
    }


    // ADD

    public async Task AddAsync(
        Order order)
    {
        await _context.Orders
            .AddAsync(order);
    }


    // DELETE

    public void Delete(
        Order order)
    {
        _context.Orders
            .Remove(order);
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