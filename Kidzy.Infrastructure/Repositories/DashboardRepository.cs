using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Domain.Enums;
using Kidzy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kidzy.Infrastructure.Repositories;

public class DashboardRepository
    : IDashboardRepository
{
    private readonly ApplicationDbContext _context;

    public DashboardRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }


    // TOTAL USERS

    public async Task<int> GetTotalUsersAsync()
    {
        return await _context.Users
            .CountAsync();
    }


    // TOTAL PRODUCTS

    public async Task<int> GetTotalProductsAsync()
    {
        return await _context.Products
            .CountAsync();
    }


    // TOTAL ORDERS

    public async Task<int> GetTotalOrdersAsync()
    {
        return await _context.Orders
            .CountAsync();
    }


    // TOTAL REVENUE

    public async Task<decimal> GetTotalRevenueAsync()
    {
        return await _context.Orders
            .Where(o =>
                o.Status == OrderStatus.Delivered)
            .SumAsync(o => o.TotalAmount);
    }


    // ORDERS BY STATUS

    public async Task<int> GetOrdersByStatusAsync(
        OrderStatus status)
    {
        return await _context.Orders
            .CountAsync(o =>
                o.Status == status);
    }
}