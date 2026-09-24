using Kidzy.Application.DTOs.Admin;

namespace Kidzy.Application.Interfaces.Services;

public interface IAdminOrderService
{
    Task<List<AdminOrderResponseDto>>
        GetAllOrdersAsync();

    Task<AdminOrderResponseDto?>
        GetOrderByIdAsync(
            int orderId);

    Task<AdminOrderResponseDto?>
        UpdateStatusAsync(
            int orderId,
            UpdateOrderStatusDto dto);

    Task<AdminOrderResponseDto?>
        CancelOrderAsync(
            int orderId);

    Task<bool>
        DeleteOrderAsync(
            int orderId);
}