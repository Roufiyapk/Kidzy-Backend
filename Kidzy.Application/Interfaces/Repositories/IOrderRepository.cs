using Kidzy.Domain.Entities;

namespace Kidzy.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> CreateAsync(Order order);

        Task<List<Order>> GetByUserIdAsync(int userId);

        Task<Order?> GetByIdAsync(
            int orderId,
            int userId);
    }
}