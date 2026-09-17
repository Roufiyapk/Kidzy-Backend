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
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;

        public CheckoutService(
            ICartRepository cartRepository,
            IProductRepository productRepository,
            IOrderRepository orderRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }

        // CART CHECKOUT

        public async Task<OrderResponseDto?> CheckoutAsync(
            int userId,
            CheckoutDto dto)
        {
            // Get current user's cart
            var cart =
                await _cartRepository.GetByUserIdAsync(userId);

            // Check cart is empty
            if (cart == null || !cart.CartItems.Any())
            {
                return null;
            }

            // Create Order
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

            decimal totalPrice = 0;

            // Process every cart item
            foreach (var cartItem in cart.CartItems)
            {
                // Get product
                var product =
                    await _productRepository
                        .GetByIdAsync(cartItem.ProductId);

                if (product == null)
                {
                    return null;
                }

                // Find selected size
                var productSize =
                    product.ProductSizes
                        .FirstOrDefault(
                            ps => ps.Size ==
                                  cartItem.SelectedSize);

                if (productSize == null)
                {
                    return null;
                }

                // Check stock
                if (productSize.Stock <
                    cartItem.Quantity)
                {
                    return null;
                }

                // Calculate subtotal
                var subTotal =
                    product.Price *
                    cartItem.Quantity;

                // Create OrderItem
                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Image = product.Image,
                    SelectedSize = cartItem.SelectedSize,
                    Quantity = cartItem.Quantity,
                    SubTotal = subTotal
                };

                order.OrderItems.Add(orderItem);

                // Add to total
                totalPrice += subTotal;

                // Reduce stock
                productSize.Stock -= cartItem.Quantity;

                await _productRepository
                    .UpdateProductSizeAsync(productSize);
            }

            // Set total
            order.TotalPrice = totalPrice;

            // Create order
            var createdOrder =
                await _orderRepository
                    .CreateAsync(order);

            // Clear cart AFTER order creation
            await _cartRepository.ClearAsync(cart);

            // Return response
            return MapToDto(createdOrder);
        }


        // BUY NOW CHECKOUT
        

        public async Task<OrderResponseDto?> BuyNowAsync(
            int userId,
            BuyNowDto dto)
        {
            // Get product
            var product =
                await _productRepository
                    .GetByIdAsync(dto.ProductId);

            // Product not found
            if (product == null)
            {
                return null;
            }

            // Find selected size
            var productSize =
                product.ProductSizes
                    .FirstOrDefault(
                        ps => ps.Size ==
                              dto.SelectedSize);

            // Size not found
            if (productSize == null)
            {
                return null;
            }

            // Check stock
            if (productSize.Stock <
                dto.Quantity)
            {
                return null;
            }

            // Calculate subtotal
            var subTotal =
                product.Price *
                dto.Quantity;

            // Create Order
            var order = new Order
            {
                UserId = userId,
                TotalPrice = subTotal,
                CustomerName = dto.CustomerName,
                Address = dto.Address,
                Phone = dto.Phone,
                PaymentMethod = "COD",
                PaymentStatus = "Pending",
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            // Create OrderItem
            var orderItem = new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Image = product.Image,
                SelectedSize = dto.SelectedSize,
                Quantity = dto.Quantity,
                SubTotal = subTotal
            };

            // Add item to order
            order.OrderItems.Add(orderItem);

            // Reduce stock
            productSize.Stock -= dto.Quantity;

            await _productRepository
                .UpdateProductSizeAsync(productSize);

            // Create order
            var createdOrder =
                await _orderRepository
                    .CreateAsync(order);

            // Buy Now does NOT clear the cart.

            return MapToDto(createdOrder);
        }


        // MAP ORDER TO RESPONSE DTO

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