using Kidzy.Domain.Entities;

namespace Kidzy.Application.Interfaces.Repositories;

public interface IOrderRepository
{
    Task<Cart?> GetCartAsync(
        int userId);

    Task<Product?> GetProductWithVariantsAsync(
        int productId);

    Task<List<Order>> GetUserOrdersAsync(
        int userId);

    Task<Order?> GetUserOrderByIdAsync(
        int userId,
        int orderId);

    Task<bool> IsPaymentAlreadyUsedAsync(
        string paymentId);

    Task AddAsync(
        Order order);

    void RemoveCartItems(
        IEnumerable<CartItem> items);

    Task SaveChangesAsync();
}