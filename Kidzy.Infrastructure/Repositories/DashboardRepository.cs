using Kidzy.Application.DTOs.Dashboard;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Domain.Enums;
using Kidzy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kidzy.Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _context;

    public DashboardRepository(ApplicationDbContext context)
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
            .Where(o => o.Status == OrderStatus.Delivered)
            .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;
    }

    // ORDERS BY STATUS
    public async Task<int> GetOrdersByStatusAsync(OrderStatus status)
    {
        return await _context.Orders
            .CountAsync(o => o.Status == status);
    }

    // MONTHLY REVENUE
    public async Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync()
    {
        var currentYear = DateTime.UtcNow.Year;

        var monthlyData = await _context.Orders
            .Where(o =>
                o.Status == OrderStatus.Delivered &&
                o.CreatedAt.Year == currentYear)
            .GroupBy(o => o.CreatedAt.Month)
            .Select(g => new
            {
                MonthNumber = g.Key,
                Revenue = g.Sum(o => o.TotalAmount)
            })
            .ToListAsync();

        var result = Enumerable.Range(1, 12)
            .Select(month =>
            {
                var data = monthlyData.FirstOrDefault(
                    x => x.MonthNumber == month);

                return new MonthlyRevenueDto
                {
                    Month = new DateTime(currentYear, month, 1)
                        .ToString("MMM"),
                    Revenue = data?.Revenue ?? 0m
                };
            })
            .ToList();

        return result;
    }

    // MONTHLY ORDERS
    public async Task<List<MonthlyOrderDto>> GetMonthlyOrdersAsync()
    {
        var currentYear = DateTime.UtcNow.Year;

        var monthlyData = await _context.Orders
            .Where(o => o.CreatedAt.Year == currentYear)
            .GroupBy(o => o.CreatedAt.Month)
            .Select(g => new
            {
                MonthNumber = g.Key,
                Orders = g.Count()
            })
            .ToListAsync();

        var result = Enumerable.Range(1, 12)
            .Select(month =>
            {
                var data = monthlyData.FirstOrDefault(
                    x => x.MonthNumber == month);

                return new MonthlyOrderDto
                {
                    Month = new DateTime(currentYear, month, 1)
                        .ToString("MMM"),
                    Orders = data?.Orders ?? 0
                };
            })
            .ToList();

        return result;
    }
}