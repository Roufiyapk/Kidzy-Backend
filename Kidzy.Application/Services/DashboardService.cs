using Kidzy.Application.DTOs.Dashboard;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;
using Kidzy.Domain.Enums;

namespace Kidzy.Application.Services;

public class DashboardService
    : IDashboardService
{
    private readonly IDashboardRepository
        _dashboardRepository;

    public DashboardService(
        IDashboardRepository dashboardRepository)
    {
        _dashboardRepository =
            dashboardRepository;
    }


    public async Task<DashboardDto>
        GetDashboardAsync()
    {
        var totalUsers =
            await _dashboardRepository
                .GetTotalUsersAsync();

        var totalProducts =
            await _dashboardRepository
                .GetTotalProductsAsync();

        var totalOrders =
            await _dashboardRepository
                .GetTotalOrdersAsync();

        var totalRevenue =
            await _dashboardRepository
                .GetTotalRevenueAsync();

        var orderPlaced =
            await _dashboardRepository
                .GetOrdersByStatusAsync(
                    OrderStatus.OrderPlaced);

        var processing =
            await _dashboardRepository
                .GetOrdersByStatusAsync(
                    OrderStatus.Processing);

        var shipped =
            await _dashboardRepository
                .GetOrdersByStatusAsync(
                    OrderStatus.Shipped);

        var outForDelivery =
            await _dashboardRepository
                .GetOrdersByStatusAsync(
                    OrderStatus.OutForDelivery);

        var delivered =
            await _dashboardRepository
                .GetOrdersByStatusAsync(
                    OrderStatus.Delivered);

        var cancelled =
            await _dashboardRepository
                .GetOrdersByStatusAsync(
                    OrderStatus.Cancelled);


        return new DashboardDto
        {
            TotalUsers = totalUsers,

            TotalProducts = totalProducts,

            TotalOrders = totalOrders,

            TotalRevenue = totalRevenue,

            OrderPlaced = orderPlaced,

            Processing = processing,

            Shipped = shipped,

            OutForDelivery = outForDelivery,

            Delivered = delivered,

            Cancelled = cancelled
        };
    }
}