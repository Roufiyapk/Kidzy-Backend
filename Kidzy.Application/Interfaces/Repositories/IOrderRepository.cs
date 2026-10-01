using Kidzy.Domain.Entities;
using Kidzy.Domain.Enums;

namespace Kidzy.Application.Interfaces.Repositories;

public interface IOrderRepository
{
    // USER

    Task<Order?>
        GetOrderByIdAsync(
            int orderId);

    Task<List<Order>>
        GetUserOrdersAsync(
            int userId);


    // ADMIN

    Task<List<Order>>
        GetAllOrdersAsync(
            OrderStatus? status = null);


    // CREATE

    Task AddAsync(
        Order order);


    // DELETE

    void Delete(
        Order order);


    // SAVE

    Task SaveChangesAsync();
}