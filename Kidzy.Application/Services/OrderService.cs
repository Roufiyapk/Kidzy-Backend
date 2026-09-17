using Kidzy.Application.DTOs.Order;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;

namespace Kidzy.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(
            IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<OrderResponseDto>>
            GetMyOrdersAsync(int userId)
        {
            var orders =
                await _orderRepository
                    .GetByUserIdAsync(userId);

            return orders
                .Select(order => new OrderResponseDto
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    TotalPrice = order.TotalPrice,
                    CustomerName = order.CustomerName,
                    Address = order.Address,
                    Phone = order.Phone,
                    PaymentMethod = order.PaymentMethod,
                    PaymentStatus = order.PaymentStatus,
                    Status = order.Status,
                    CreatedAt = order.CreatedAt,

                    Items = order.OrderItems
                        .Select(item =>
                            new OrderItemResponseDto
                            {
                                Id = item.Id,
                                ProductId = item.ProductId,
                                ProductName =
                                    item.ProductName,
                                UnitPrice =
                                    item.UnitPrice,
                                Image = item.Image,
                                SelectedSize =
                                    item.SelectedSize,
                                Quantity =
                                    item.Quantity,
                                SubTotal =
                                    item.SubTotal
                            })
                        .ToList()
                })
                .ToList();
        }
    }
}