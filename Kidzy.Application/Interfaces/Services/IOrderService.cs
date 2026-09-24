using Kidzy.Application.DTOs.Orders;

namespace Kidzy.Application.Interfaces.Services;

public interface IOrderService
{
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
}