using System.Text.RegularExpressions;
using Kidzy.Application.DTOs.Orders;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;
using Kidzy.Domain.Entities;
using Kidzy.Domain.Enums;

namespace Kidzy.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(
        IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }


    // ORDER FROM CART

    public async Task<OrderResponseDto>
        CreateOrderFromCartAsync(
            int userId,
            CreateOrderDto dto)
    {
        ValidateDeliveryDetails(dto);

        var cart =
            await _orderRepository
                .GetCartAsync(userId);

        if (cart == null ||
            !cart.Items.Any())
        {
            throw new Exception(
                "Cart is empty.");
        }


        // Check stock before creating order

        foreach (var cartItem in cart.Items)
        {
            CheckStock(
                cartItem.Product,
                cartItem.ProductVariant,
                cartItem.Quantity);
        }


        // Calculate totals

        var subtotal =
            cart.Items.Sum(
                item =>
                    item.Product.Price *
                    item.Quantity);

        var deliveryFee =
            CalculateDeliveryFee(subtotal);

        var totalAmount =
            subtotal + deliveryFee;


        // Create order

        var order = new Order
        {
            UserId = userId,

            Subtotal = subtotal,

            DeliveryFee = deliveryFee,

            TotalAmount = totalAmount,

            ShippingName =
                dto.Name.Trim(),

            ShippingPhone =
                dto.Phone.Trim(),

            ShippingAddress =
                dto.Address.Trim(),

            ShippingPincode =
                dto.Pincode.Trim(),

            PaymentMethod =
                dto.PaymentMethod
                    .Trim()
                    .ToLowerInvariant(),

            PaymentStatus =
                GetPaymentStatus(
                    dto.PaymentMethod),

            RazorpayPaymentId =
                dto.RazorpayPaymentId,

            RazorpayOrderId =
                dto.RazorpayOrderId,

            RazorpaySignature =
                dto.RazorpaySignature,

            Status =
                OrderStatus.OrderPlaced,

            CreatedAt =
                DateTime.UtcNow
        };


        // Create order items

        foreach (var cartItem in cart.Items)
        {
            var variant =
                cartItem.ProductVariant;

            var orderItem = new OrderItem
            {
                ProductId =
                    cartItem.ProductId,

                ProductVariantId =
                    cartItem.ProductVariantId,

                ProductName =
                    cartItem.Product.Name,

                AgeGroup =
                    variant?.AgeGroup,

                Size =
                    variant?.Size,

                Price =
                    cartItem.Product.Price,

                Quantity =
                    cartItem.Quantity,

                TotalPrice =
                    cartItem.Product.Price *
                    cartItem.Quantity
            };

            order.Items.Add(orderItem);


            // Reduce stock

            if (variant != null)
            {
                variant.Stock -=
                    cartItem.Quantity;
            }
        }


        await _orderRepository
            .AddAsync(order);


        // Cart order → remove cart items

        _orderRepository
            .RemoveCartItems(cart.Items);


        await _orderRepository
            .SaveChangesAsync();


        return MapToDto(order);
    }


    // BUY NOW

    public async Task<OrderResponseDto>
        CreateBuyNowOrderAsync(
            int userId,
            BuyNowOrderDto dto)
    {
        ValidateBuyNow(dto);


        // Get selected product

        var product =
            await _orderRepository
                .GetProductWithVariantsAsync(
                    dto.ProductId);

        if (product == null)
        {
            throw new Exception(
                "Product not found.");
        }


        // Check whether product has variants

        var hasVariants =
            product.Variants != null &&
            product.Variants.Any();


        if (hasVariants &&
            dto.ProductVariantId == null)
        {
            throw new Exception(
                "Please select age group and size.");
        }


        ProductVariant? variant = null;


        // Get selected variant

        if (dto.ProductVariantId.HasValue)
        {
            variant =
                product.Variants
                    .FirstOrDefault(
                        v =>
                            v.Id ==
                            dto.ProductVariantId.Value);

            if (variant == null)
            {
                throw new Exception(
                    "Invalid product variant.");
            }

            if (dto.Quantity >
                variant.Stock)
            {
                throw new Exception(
                    "Requested quantity exceeds available stock.");
            }
        }


        // Calculate totals

        var subtotal =
            product.Price *
            dto.Quantity;

        var deliveryFee =
            CalculateDeliveryFee(subtotal);

        var totalAmount =
            subtotal + deliveryFee;


        // Create order

        var order = new Order
        {
            UserId = userId,

            Subtotal = subtotal,

            DeliveryFee = deliveryFee,

            TotalAmount = totalAmount,

            ShippingName =
                dto.Name.Trim(),

            ShippingPhone =
                dto.Phone.Trim(),

            ShippingAddress =
                dto.Address.Trim(),

            ShippingPincode =
                dto.Pincode.Trim(),

            PaymentMethod =
                dto.PaymentMethod
                    .Trim()
                    .ToLowerInvariant(),

            PaymentStatus =
                GetPaymentStatus(
                    dto.PaymentMethod),

            RazorpayPaymentId =
                dto.RazorpayPaymentId,

            RazorpayOrderId =
                dto.RazorpayOrderId,

            RazorpaySignature =
                dto.RazorpaySignature,

            Status =
                OrderStatus.OrderPlaced,

            CreatedAt =
                DateTime.UtcNow
        };


        // Create single order item

        var orderItem = new OrderItem
        {
            ProductId =
                product.Id,

            ProductVariantId =
                dto.ProductVariantId,

            ProductName =
                product.Name,

            AgeGroup =
                variant?.AgeGroup,

            Size =
                variant?.Size,

            Price =
                product.Price,

            Quantity =
                dto.Quantity,

            TotalPrice =
                product.Price *
                dto.Quantity
        };

        order.Items.Add(orderItem);


        // Reduce stock

        if (variant != null)
        {
            variant.Stock -=
                dto.Quantity;
        }


        await _orderRepository
            .AddAsync(order);

        // Buy Now does NOT clear cart.

        await _orderRepository
            .SaveChangesAsync();


        return MapToDto(order);
    }


    // GET ALL USER ORDERS

    public async Task<List<OrderResponseDto>>
        GetUserOrdersAsync(int userId)
    {
        var orders =
            await _orderRepository
                .GetUserOrdersAsync(userId);

        return orders
            .Select(MapToDto)
            .ToList();
    }


    // GET SINGLE USER ORDER

    public async Task<OrderResponseDto?>
        GetOrderByIdAsync(
            int userId,
            int orderId)
    {
        var order =
            await _orderRepository
                .GetUserOrderByIdAsync(
                    userId,
                    orderId);

        if (order == null)
            return null;

        return MapToDto(order);
    }


    // =====================================================
    // CANCEL USER ORDER
    // =====================================================

    public async Task<OrderResponseDto?>
        CancelOrderAsync(
            int userId,
            int orderId)
    {
        var order =
            await _orderRepository
                .GetUserOrderByIdAsync(
                    userId,
                    orderId);

        if (order == null)
            return null;


        // User can cancel only before shipping

        if (order.Status !=
                OrderStatus.OrderPlaced &&
            order.Status !=
                OrderStatus.Processing)
        {
            throw new Exception(
                "This order cannot be cancelled.");
        }


        // Restore stock

        foreach (var item in order.Items)
        {
            if (item.ProductVariant != null)
            {
                item.ProductVariant.Stock +=
                    item.Quantity;
            }
        }


        order.Status =
            OrderStatus.Cancelled;

        order.CancelledAt =
            DateTime.UtcNow;


        await _orderRepository
            .SaveChangesAsync();


        return MapToDto(order);
    }


    // =====================================================
    // VALIDATION
    // =====================================================

    private static void ValidateDeliveryDetails(
        CreateOrderDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new Exception(
                "Name is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Phone))
        {
            throw new Exception(
                "Phone number is required.");
        }

        if (!Regex.IsMatch(
                dto.Phone.Trim(),
                @"^\d{10}$"))
        {
            throw new Exception(
                "Phone number must be exactly 10 digits.");
        }

        if (string.IsNullOrWhiteSpace(dto.Address))
        {
            throw new Exception(
                "Address is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Pincode))
        {
            throw new Exception(
                "Pincode is required.");
        }

        if (!Regex.IsMatch(
                dto.Pincode.Trim(),
                @"^\d{6}$"))
        {
            throw new Exception(
                "Pincode must be exactly 6 digits.");
        }

        ValidatePaymentMethod(
            dto.PaymentMethod);
    }


    private static void ValidateBuyNow(
        BuyNowOrderDto dto)
    {
        if (dto.ProductId <= 0)
        {
            throw new Exception(
                "Invalid product.");
        }

        if (dto.Quantity <= 0)
        {
            throw new Exception(
                "Quantity must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new Exception(
                "Name is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Phone))
        {
            throw new Exception(
                "Phone number is required.");
        }

        if (!Regex.IsMatch(
                dto.Phone.Trim(),
                @"^\d{10}$"))
        {
            throw new Exception(
                "Phone number must be exactly 10 digits.");
        }

        if (string.IsNullOrWhiteSpace(dto.Address))
        {
            throw new Exception(
                "Address is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Pincode))
        {
            throw new Exception(
                "Pincode is required.");
        }

        if (!Regex.IsMatch(
                dto.Pincode.Trim(),
                @"^\d{6}$"))
        {
            throw new Exception(
                "Pincode must be exactly 6 digits.");
        }

        ValidatePaymentMethod(
            dto.PaymentMethod);
    }


    private static void ValidatePaymentMethod(
        string paymentMethod)
    {
        if (string.IsNullOrWhiteSpace(paymentMethod))
        {
            throw new Exception(
                "Payment method is required.");
        }

        var method =
            paymentMethod
                .Trim()
                .ToLowerInvariant();

        if (method != "cod" &&
            method != "razorpay")
        {
            throw new Exception(
                "Invalid payment method.");
        }
    }


    // =====================================================
    // STOCK
    // =====================================================

    private static void CheckStock(
        Product product,
        ProductVariant? variant,
        int quantity)
    {
        if (variant != null)
        {
            if (variant.Stock <= 0)
            {
                throw new Exception(
                    $"{product.Name} - " +
                    $"{variant.Size} is out of stock.");
            }

            if (quantity > variant.Stock)
            {
                throw new Exception(
                    $"Only {variant.Stock} available for " +
                    $"{product.Name} ({variant.Size}).");
            }
        }
    }


    // =====================================================
    // DELIVERY FEE
    // =====================================================

    private static decimal CalculateDeliveryFee(
        decimal subtotal)
    {
        return subtotal < 499
            ? 100
            : 0;
    }


    // =====================================================
    // PAYMENT STATUS
    // =====================================================

    private static string GetPaymentStatus(
        string paymentMethod)
    {
        return paymentMethod
            .Trim()
            .ToLowerInvariant() == "razorpay"
                ? "Paid"
                : "Pending";
    }


    // =====================================================
    // MAP TO RESPONSE DTO
    // =====================================================

    private static OrderResponseDto MapToDto(
        Order order)
    {
        return new OrderResponseDto
        {
            Id =
                order.Id,

            Subtotal =
                order.Subtotal,

            DeliveryFee =
                order.DeliveryFee,

            TotalAmount =
                order.TotalAmount,

            Name =
                order.ShippingName,

            Phone =
                order.ShippingPhone,

            Address =
                order.ShippingAddress,

            Pincode =
                order.ShippingPincode,

            PaymentMethod =
                order.PaymentMethod,

            PaymentStatus =
                order.PaymentStatus,

            RazorpayPaymentId =
                order.RazorpayPaymentId,

            RazorpayOrderId =
                order.RazorpayOrderId,

            RazorpaySignature =
                order.RazorpaySignature,

            Status =
                order.Status.ToString(),

            CreatedAt =
                order.CreatedAt,

            CancelledAt =
                order.CancelledAt,

            Items =
                order.Items
                    .Select(
                        item =>
                            new OrderItemDto
                            {
                                Id =
                                    item.Id,

                                ProductId =
                                    item.ProductId,

                                ProductVariantId =
                                    item.ProductVariantId,

                                ProductName =
                                    item.ProductName,

                                AgeGroup =
                                    item.AgeGroup,

                                Size =
                                    item.Size,

                                Price =
                                    item.Price,

                                Quantity =
                                    item.Quantity,

                                TotalPrice =
                                    item.TotalPrice
                            })
                    .ToList()
        };
    }
}