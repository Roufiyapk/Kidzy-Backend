using Kidzy.Application.DTOs.Order;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;
using Kidzy.Domain.Entities;

namespace Kidzy.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
        }

        public async Task<OrderResponseDto?> CreateOrderAsync(
            int userId,
            CreateOrderDto dto)
        {
            var cart =
                await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null ||
                cart.CartItems == null ||
                !cart.CartItems.Any())
            {
                return null;
            }

            var order = new Order
            {
                UserId = userId,

                CustomerName = dto.CustomerName,

                Address = dto.Address,

                Phone = dto.Phone,

                PaymentMethod = dto.PaymentMethod,

                PaymentStatus = "Pending",

                Status = "Pending",

                CreatedAt = DateTime.UtcNow
            };

            foreach (var cartItem in cart.CartItems)
            {
                var subTotal =
                    cartItem.Product.Price *
                    cartItem.Quantity;

                var orderItem = new OrderItem
                {
                    ProductId = cartItem.ProductId,

                    ProductName = cartItem.Product.Name,

                    UnitPrice = cartItem.Product.Price,

                    Image = cartItem.Product.Image,

                    SelectedSize = cartItem.SelectedSize,

                    Quantity = cartItem.Quantity,

                    SubTotal = subTotal
                };

                order.OrderItems.Add(orderItem);
            }

            order.TotalPrice =
                order.OrderItems.Sum(
                    item => item.SubTotal);

            var createdOrder =
                await _orderRepository.CreateAsync(order);

            // Clear cart after order creation
            await _cartRepository.ClearAsync(cart);

            return MapToDto(createdOrder);
        }

        public async Task<List<OrderResponseDto>> GetMyOrdersAsync(
            int userId)
        {
            var orders =
                await _orderRepository.GetByUserIdAsync(userId);

            return orders
                .Select(MapToDto)
                .ToList();
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(
            int userId,
            int orderId)
        {
            var order =
                await _orderRepository.GetByIdAsync(
                    orderId,
                    userId);

            if (order == null)
            {
                return null;
            }

            return MapToDto(order);
        }

        private static OrderResponseDto MapToDto(
            Order order)
        {
            return new OrderResponseDto
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
                    .Select(item => new OrderItemResponseDto
                    {
                        Id = item.Id,

                        ProductId = item.ProductId,

                        ProductName = item.ProductName,

                        UnitPrice = item.UnitPrice,

                        Image = item.Image,

                        SelectedSize = item.SelectedSize,

                        Quantity = item.Quantity,

                        SubTotal = item.SubTotal
                    })
                    .ToList()
            };
        }
    }
}