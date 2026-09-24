using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Domain.Entities;
using Kidzy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kidzy.Infrastructure.Repositories;

public class AdminOrderRepository
    : IAdminOrderRepository
{
    private readonly ApplicationDbContext _context;

    public AdminOrderRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }


    // GET ALL ORDERS

    public async Task<List<Order>>
        GetAllOrdersAsync()
    {
        return await _context.Orders

            .Include(o => o.User)

            .Include(o => o.Items)
                .ThenInclude(i => i.Product)

            .Include(o => o.Items)
                .ThenInclude(i => i.ProductVariant)

            .OrderByDescending(
                o => o.CreatedAt)

            .AsNoTracking()

            .ToListAsync();
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


    // DELETE

    public void Delete(Order order)
    {
        _context.Orders.Remove(order);
    }


    // SAVE

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}