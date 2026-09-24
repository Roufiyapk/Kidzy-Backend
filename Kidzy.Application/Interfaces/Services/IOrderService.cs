using Kidzy.Application.DTOs.Orders;

namespace Kidzy.Application.Interfaces.Services;

public interface IOrderService
{
    Task<OrderResponseDto> CreateOrderFromCartAsync(
        int userId,
        CreateOrderDto dto);

    Task<OrderResponseDto> CreateBuyNowOrderAsync(
        int userId,
        BuyNowOrderDto dto);

    Task<List<OrderResponseDto>> GetUserOrdersAsync(
        int userId);

    Task<OrderResponseDto?> GetOrderByIdAsync(
        int userId,
        int orderId);

    Task<OrderResponseDto?> CancelOrderAsync(
        int userId,
        int orderId);
}