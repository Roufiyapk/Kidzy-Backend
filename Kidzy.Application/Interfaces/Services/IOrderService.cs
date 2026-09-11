using Kidzy.Application.DTOs.Order;

namespace Kidzy.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<OrderResponseDto?> CreateOrderAsync(
            int userId,
            CreateOrderDto dto);

        Task<List<OrderResponseDto>> GetMyOrdersAsync(
            int userId);

        Task<OrderResponseDto?> GetOrderByIdAsync(
            int userId,
            int orderId);
    }
}