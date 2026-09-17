using Kidzy.Application.DTOs.Order;

namespace Kidzy.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<List<OrderResponseDto>> GetMyOrdersAsync(int userId);
    }
}