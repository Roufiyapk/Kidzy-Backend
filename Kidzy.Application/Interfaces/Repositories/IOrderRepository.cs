using Kidzy.Domain.Entities;
using Kidzy.Domain.Enums;

namespace Kidzy.Application.Interfaces.Repositories;

public interface IOrderRepository
{
    Task<Cart?>
        GetCartAsync(
            int userId);


    Task<Product?>
        GetProductWithVariantsAsync(
            int productId);


    Task<List<Order>>
        GetUserOrdersAsync(
            int userId);


    Task<Order?>
        GetOrderByIdAsync(
            int orderId);


    Task<List<Order>>
        GetAllOrdersAsync(
            OrderStatus? status = null);


    Task AddAsync(
        Order order);


    void Delete(
        Order order);


    void RemoveCartItems(
        IEnumerable<CartItem> items);


    Task SaveChangesAsync();
}