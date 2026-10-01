using Kidzy.Application.DTOs.Orders;

namespace Kidzy.Application.Interfaces.Services;

public interface IOrderService
{
    // USER

    Task<CheckoutResponseDto>
        CreateOrderFromCartAsync(
            int userId,
            CreateOrderDto dto);


    Task<CheckoutResponseDto>
        CreateBuyNowOrderAsync(
            int userId,
            BuyNowOrderDto dto);


    Task<List<OrderResponseDto>>
        GetUserOrdersAsync(
            int userId);


    Task<OrderResponseDto?>
        GetOrderByIdAsync(
            int userId,
            int orderId);


    Task<OrderResponseDto?>
        CancelOrderAsync(
            int userId,
            int orderId);


    // ADMIN

    Task<List<AdminOrderResponseDto>>
        GetAllOrdersAsync(
            string? status = null);


    Task<AdminOrderResponseDto?>
        GetAdminOrderByIdAsync(
            int orderId);


    Task<AdminOrderResponseDto?>
        UpdateStatusAsync(
            int orderId,
            UpdateOrderStatusDto dto);


    Task<AdminOrderResponseDto?>
        CancelAdminOrderAsync(
            int orderId);


    Task<bool>
        DeleteOrderAsync(
            int orderId);
}