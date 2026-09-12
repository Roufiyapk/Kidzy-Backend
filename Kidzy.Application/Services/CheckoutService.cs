using Kidzy.Application.DTOs.Checkout;
using Kidzy.Application.DTOs.Order;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;
using Kidzy.Domain.Entities;

namespace Kidzy.Application.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;

        public CheckoutService(
            ICartRepository cartRepository,
            IOrderRepository orderRepository)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
        }

        public async Task<OrderResponseDto?> CheckoutAsync(
            int userId,
            CheckoutDto dto)
        {
            var cart =
                await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null ||
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
                PaymentMethod = "COD",
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

            await _cartRepository.ClearAsync(cart);

            return MapToDto(createdOrder);
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
                    .Select(item =>
                        new OrderItemResponseDto
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