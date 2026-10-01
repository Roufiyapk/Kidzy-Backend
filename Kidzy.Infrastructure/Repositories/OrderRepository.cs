using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Domain.Entities;
using Kidzy.Domain.Enums;
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


    // GET ONE ORDER

    public async Task<Order?>
        GetOrderByIdAsync(
            int orderId)
    {
        return await _context.Orders

            .Include(o => o.User)

            .Include(o => o.Items)
                .ThenInclude(i => i.Product)

            .Include(o => o.Items)
                .ThenInclude(i => i.ProductVariant)

            .FirstOrDefaultAsync(
                o => o.Id == orderId);
    }


    // GET USER ORDERS

    public async Task<List<Order>>
        GetUserOrdersAsync(
            int userId)
    {
        return await _context.Orders

            .Include(o => o.Items)
                .ThenInclude(i => i.Product)

            .Include(o => o.Items)
                .ThenInclude(i => i.ProductVariant)

            .Where(o =>
                o.UserId == userId)

            .OrderByDescending(
                o => o.CreatedAt)

            .AsNoTracking()

            .ToListAsync();
    }


    // GET ALL ORDERS
    // ADMIN
   

    public async Task<List<Order>>
        GetAllOrdersAsync(
            OrderStatus? status = null)
    {
        var query =
            _context.Orders

                .Include(o => o.User)

                .Include(o => o.Items)
                    .ThenInclude(i =>
                        i.Product)

                .Include(o => o.Items)
                    .ThenInclude(i =>
                        i.ProductVariant)

                .AsQueryable();


        // STATUS FILTER

        if (status.HasValue)
        {
            query = query.Where(
                o => o.Status == status.Value);
        }


        return await query

            .OrderByDescending(
                o => o.CreatedAt)

            .AsNoTracking()

            .ToListAsync();
    }


    // ADD ORDER

    public async Task AddAsync(
        Order order)
    {
        await _context.Orders
            .AddAsync(order);
    }


    // DELETE ORDER

    public void Delete(
        Order order)
    {
        _context.Orders.Remove(order);
    }


    // SAVE CHANGES

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}