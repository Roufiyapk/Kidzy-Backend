using Kidzy.Application.DTOs.Dashboard;
using Kidzy.Domain.Enums;

namespace Kidzy.Application.Interfaces.Repositories;

public interface IDashboardRepository
{
    Task<int> GetTotalUsersAsync();

    Task<int> GetTotalProductsAsync();

    Task<int> GetTotalOrdersAsync();

    Task<decimal> GetTotalRevenueAsync();

    Task<int> GetOrdersByStatusAsync(OrderStatus status);

    Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync();

    Task<List<MonthlyOrderDto>> GetMonthlyOrdersAsync();
}