using Kidzy.Domain.Entities;

namespace Kidzy.Application.Interfaces.Repositories;

public interface IAdminOrderRepository
{
    Task<List<Order>> GetAllOrdersAsync();

    Task<Order?> GetOrderByIdAsync(
        int orderId);

    void Delete(Order order);

    Task SaveChangesAsync();
}