using Kidzy.Domain.Entities;
using Kidzy.Domain.Enums;

namespace Kidzy.Application.Interfaces.Repositories;

public interface IOrderRepository
{

    Task<Order?>
        GetOrderByIdAsync(
            int orderId);

    Task<List<Order>>
        GetUserOrdersAsync(
            int userId);



    Task<List<Order>>
        GetAllOrdersAsync(
            OrderStatus? status = null);



    Task AddAsync(
        Order order);



    void Delete(
        Order order);



    Task SaveChangesAsync();
}