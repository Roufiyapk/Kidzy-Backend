using Kidzy.Application.DTOs.Orders;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;

using Kidzy.Domain.Entities;
using Kidzy.Domain.Enums;

namespace Kidzy.Application.Services;

public class OrderService
    : IOrderService
{
    private readonly IOrderRepository
        _orderRepository;

    private readonly IPaymentRepository
        _paymentRepository;


    public OrderService(
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository)
    {
        _orderRepository =
            orderRepository;

        _paymentRepository =
            paymentRepository;
    }


    // CART ORDER

    public async Task<OrderResponseDto>
        CreateOrderFromCartAsync(
            int userId,
            CreateOrderDto dto)
    {
        ValidateShipping(dto);


        var paymentMethod =
            dto.PaymentMethod
                .Trim()
                .ToLowerInvariant();


        if (paymentMethod != "cod" &&
            paymentMethod != "razorpay")
        {
            throw new Exception(
                "Invalid payment method.");
        }


        var cart =
            await _orderRepository
                .GetCartAsync(
                    userId);


        if (cart == null ||
            cart.Items == null ||
            !cart.Items.Any())
        {
            throw new Exception(
                "Cart is empty.");
        }


        // STOCK CHECK

        foreach (var item in cart.Items)
        {
            if (item.Product == null)
            {
                throw new Exception(
                    "Product not found in cart.");
            }


            CheckStock(
                item.Product,
                item.ProductVariant,
                item.Quantity);
        }


        // CALCULATE TOTAL

        var subtotal =
            cart.Items.Sum(
                item =>
                    item.Product.Price *
                    item.Quantity);


        var deliveryFee =
            CalculateDeliveryFee(
                subtotal);


        var total =
            subtotal +
            deliveryFee;


        // RAZORPAY PAYMENT CHECK

        Payment? payment = null;


        if (paymentMethod ==
            "razorpay")
        {
            if (!dto.PaymentId.HasValue)
            {
                throw new Exception(
                    "Verified payment is required.");
            }


            payment =
                await _paymentRepository
                    .GetByIdAsync(
                        dto.PaymentId.Value);


            ValidatePayment(
                payment,
                userId,
                total,
                "cart");
        }


        // CREATE ORDER

        var order =
            new Order
            {
                UserId =
                    userId,

                PaymentId =
                    payment?.Id,

                Subtotal =
                    subtotal,

                DeliveryFee =
                    deliveryFee,

                TotalAmount =
                    total,

                ShippingName =
                    dto.Name.Trim(),

                ShippingPhone =
                    dto.Phone.Trim(),

                ShippingAddress =
                    dto.Address.Trim(),

                ShippingPlace =
                    dto.Place.Trim(),

                ShippingState =
                    dto.State.Trim(),

                ShippingPincode =
                    dto.Pincode.Trim(),

                PaymentMethod =
                    paymentMethod,

                PaymentStatus =
                    paymentMethod ==
                    "razorpay"
                        ? "Paid"
                        : "Pending",

                RazorpayPaymentId =
                    payment?.RazorpayPaymentId,

                RazorpayOrderId =
                    payment?.RazorpayOrderId,

                RazorpaySignature =
                    payment?.RazorpaySignature,

                Status =
                    OrderStatus.OrderPlaced,

                CreatedAt =
                    DateTime.UtcNow
            };


        // ITEMS + STOCK

        foreach (var cartItem in cart.Items)
        {
            var variant =
                cartItem.ProductVariant;


            var orderItem =
                new OrderItem
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


            order.Items.Add(
                orderItem);


            if (variant != null)
            {
                variant.Stock -=
                    cartItem.Quantity;
            }
            else
            {
                cartItem.Product.Stock -=
                    cartItem.Quantity;
            }
        }


        // SAVE ORDER

        await _orderRepository
            .AddAsync(
                order);


        // CLEAR CART

        _orderRepository
            .RemoveCartItems(
                cart.Items);


        await _orderRepository
            .SaveChangesAsync();


        return MapToUserDto(
            order);
    }


    // BUY NOW

    public async Task<OrderResponseDto>
        CreateBuyNowOrderAsync(
            int userId,
            BuyNowOrderDto dto)
    {
        ValidateShipping(
            dto.Name,
            dto.Address,
            dto.Place,
            dto.State,
            dto.Pincode,
            dto.Phone,
            dto.PaymentMethod);


        if (dto.Quantity <= 0)
        {
            throw new Exception(
                "Quantity must be greater than zero.");
        }


        var paymentMethod =
            dto.PaymentMethod
                .Trim()
                .ToLowerInvariant();


        if (paymentMethod != "cod" &&
            paymentMethod != "razorpay")
        {
            throw new Exception(
                "Invalid payment method.");
        }


        // PRODUCT

        var product =
            await _orderRepository
                .GetProductWithVariantsAsync(
                    dto.ProductId);


        if (product == null)
        {
            throw new Exception(
                "Product not found.");
        }


        // VARIANT

        var variant =
            GetVariant(
                product,
                dto.ProductVariantId);


        // STOCK

        CheckStock(
            product,
            variant,
            dto.Quantity);


        // AMOUNT

        var subtotal =
            product.Price *
            dto.Quantity;


        var deliveryFee =
            CalculateDeliveryFee(
                subtotal);


        var total =
            subtotal +
            deliveryFee;


        // PAYMENT

        Payment? payment = null;


        if (paymentMethod ==
            "razorpay")
        {
            if (!dto.PaymentId.HasValue)
            {
                throw new Exception(
                    "Verified payment is required.");
            }


            payment =
                await _paymentRepository
                    .GetByIdAsync(
                        dto.PaymentId.Value);


            ValidatePayment(
                payment,
                userId,
                total,
                "buy-now");


            if (payment.ProductId !=
                dto.ProductId)
            {
                throw new Exception(
                    "Payment does not match the product.");
            }


            if (payment.ProductVariantId !=
                dto.ProductVariantId)
            {
                throw new Exception(
                    "Payment does not match the selected variant.");
            }


            if (payment.Quantity !=
                dto.Quantity)
            {
                throw new Exception(
                    "Payment does not match the quantity.");
            }
        }


        // ORDER

        var order =
            new Order
            {
                UserId =
                    userId,

                PaymentId =
                    payment?.Id,

                Subtotal =
                    subtotal,

                DeliveryFee =
                    deliveryFee,

                TotalAmount =
                    total,

                ShippingName =
                    dto.Name.Trim(),

                ShippingPhone =
                    dto.Phone.Trim(),

                ShippingAddress =
                    dto.Address.Trim(),

                ShippingPlace =
                    dto.Place.Trim(),

                ShippingState =
                    dto.State.Trim(),

                ShippingPincode =
                    dto.Pincode.Trim(),

                PaymentMethod =
                    paymentMethod,

                PaymentStatus =
                    paymentMethod ==
                    "razorpay"
                        ? "Paid"
                        : "Pending",

                RazorpayPaymentId =
                    payment?.RazorpayPaymentId,

                RazorpayOrderId =
                    payment?.RazorpayOrderId,

                RazorpaySignature =
                    payment?.RazorpaySignature,

                Status =
                    OrderStatus.OrderPlaced,

                CreatedAt =
                    DateTime.UtcNow
            };


        // ORDER ITEM

        order.Items.Add(
            new OrderItem
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
            });


        // REDUCE STOCK

        if (variant != null)
        {
            variant.Stock -=
                dto.Quantity;
        }
        else
        {
            product.Stock -=
                dto.Quantity;
        }


        // SAVE

        await _orderRepository
            .AddAsync(
                order);


        // BUY NOW DOES NOT CLEAR CART

        await _orderRepository
            .SaveChangesAsync();


        return MapToUserDto(
            order);
    }


    // USER ORDERS

    public async Task<List<OrderResponseDto>>
        GetUserOrdersAsync(
            int userId)
    {
        var orders =
            await _orderRepository
                .GetUserOrdersAsync(
                    userId);


        return orders
            .Select(
                MapToUserDto)
            .ToList();
    }


    // ONE USER ORDER

    public async Task<OrderResponseDto?>
        GetOrderByIdAsync(
            int userId,
            int orderId)
    {
        var order =
            await _orderRepository
                .GetOrderByIdAsync(
                    orderId);


        if (order == null)
        {
            return null;
        }


        if (order.UserId !=
            userId)
        {
            return null;
        }


        return MapToUserDto(
            order);
    }


    // USER CANCEL

    public async Task<OrderResponseDto?>
        CancelOrderAsync(
            int userId,
            int orderId)
    {
        var order =
            await _orderRepository
                .GetOrderByIdAsync(
                    orderId);


        if (order == null)
        {
            return null;
        }


        if (order.UserId !=
            userId)
        {
            return null;
        }


        if (order.Status !=
                OrderStatus.OrderPlaced &&
            order.Status !=
                OrderStatus.Processing)
        {
            throw new Exception(
                "This order cannot be cancelled.");
        }


        RestoreStock(
            order);


        order.Status =
            OrderStatus.Cancelled;

        order.CancelledAt =
            DateTime.UtcNow;


        await _orderRepository
            .SaveChangesAsync();


        return MapToUserDto(
            order);
    }


    // ADMIN ALL ORDERS

    public async Task<List<AdminOrderResponseDto>>
        GetAllOrdersAsync(
            string? status = null)
    {
        OrderStatus? parsedStatus =
            null;


        if (!string.IsNullOrWhiteSpace(
                status))
        {
            parsedStatus =
                ParseStatus(
                    status);
        }


        var orders =
            await _orderRepository
                .GetAllOrdersAsync(
                    parsedStatus);


        return orders
            .Select(
                MapToAdminDto)
            .ToList();
    }


    // ADMIN ONE ORDER

    public async Task<AdminOrderResponseDto?>
        GetAdminOrderByIdAsync(
            int orderId)
    {
        var order =
            await _orderRepository
                .GetOrderByIdAsync(
                    orderId);


        if (order == null)
        {
            return null;
        }


        return MapToAdminDto(
            order);
    }
    // ADMIN STATUS

    public async Task<AdminOrderResponseDto?>
        UpdateStatusAsync(
            int orderId,
            UpdateOrderStatusDto dto)
    {
        if (dto == null ||
            string.IsNullOrWhiteSpace(
                dto.Status))
        {
            throw new Exception(
                "Order status is required.");
        }


        var order =
            await _orderRepository
                .GetOrderByIdAsync(
                    orderId);


        if (order == null)
        {
            return null;
        }


        var newStatus =
            ParseStatus(
                dto.Status);


        if (order.Status ==
                OrderStatus.Cancelled &&
            newStatus !=
                OrderStatus.Cancelled)
        {
            throw new Exception(
                "Cancelled order cannot be reopened.");
        }


        if (newStatus ==
                OrderStatus.Cancelled &&
            order.Status !=
                OrderStatus.Cancelled)
        {
            if (order.Status !=
                    OrderStatus.OrderPlaced &&
                order.Status !=
                    OrderStatus.Processing)
            {
                throw new Exception(
                    "This order cannot be cancelled at this stage.");
            }


            RestoreStock(
                order);


            order.Status =
                OrderStatus.Cancelled;

            order.CancelledAt =
                DateTime.UtcNow;
        }
        else
        {
            order.Status =
                newStatus;
        }


        await _orderRepository
            .SaveChangesAsync();


        return MapToAdminDto(
            order);
    }


    // ADMIN CANCEL

    public async Task<AdminOrderResponseDto?>
        CancelAdminOrderAsync(
            int orderId)
    {
        var order =
            await _orderRepository
                .GetOrderByIdAsync(
                    orderId);


        if (order == null)
        {
            return null;
        }


        if (order.Status !=
                OrderStatus.OrderPlaced &&
            order.Status !=
                OrderStatus.Processing)
        {
            throw new Exception(
                "This order cannot be cancelled.");
        }


        RestoreStock(
            order);


        order.Status =
            OrderStatus.Cancelled;

        order.CancelledAt =
            DateTime.UtcNow;


        await _orderRepository
            .SaveChangesAsync();


        return MapToAdminDto(
            order);
    }


    // ADMIN DELETE

    public async Task<bool>
        DeleteOrderAsync(
            int orderId)
    {
        var order =
            await _orderRepository
                .GetOrderByIdAsync(
                    orderId);


        if (order == null)
        {
            return false;
        }


        if (order.Status !=
            OrderStatus.Cancelled)
        {
            throw new Exception(
                "Only cancelled orders can be deleted.");
        }


        _orderRepository
            .Delete(
                order);


        await _orderRepository
            .SaveChangesAsync();


        return true;
    }


    // VALIDATE PAYMENT

    private static void
        ValidatePayment(
            Payment? payment,
            int userId,
            decimal expectedTotal,
            string checkoutType)
    {
        if (payment == null)
        {
            throw new Exception(
                "Payment not found.");
        }


        if (payment.UserId !=
            userId)
        {
            throw new UnauthorizedAccessException(
                "Invalid payment.");
        }


        if (payment.Status !=
            "Paid")
        {
            throw new Exception(
                "Payment has not been completed.");
        }


        if (payment.OrderId.HasValue)
        {
            throw new Exception(
                "This payment is already linked to an order.");
        }


        if (payment.CheckoutType !=
            checkoutType)
        {
            throw new Exception(
                "Payment checkout type mismatch.");
        }


        if (payment.Amount !=
            expectedTotal)
        {
            throw new Exception(
                "Payment amount does not match order amount.");
        }
    }


    // STOCK

    private static void
        CheckStock(
            Product product,
            ProductVariant? variant,
            int quantity)
    {
        if (quantity <= 0)
        {
            throw new Exception(
                "Quantity must be greater than zero.");
        }


        if (variant != null)
        {
            if (variant.Stock <= 0)
            {
                throw new Exception(
                    $"{product.Name} - {variant.Size} is out of stock.");
            }


            if (quantity >
                variant.Stock)
            {
                throw new Exception(
                    $"Only {variant.Stock} available for " +
                    $"{product.Name} ({variant.Size}).");
            }


            return;
        }


        if (product.Stock <= 0)
        {
            throw new Exception(
                $"{product.Name} is out of stock.");
        }


        if (quantity >
            product.Stock)
        {
            throw new Exception(
                $"Only {product.Stock} available for " +
                $"{product.Name}.");
        }
    }


    // VARIANT

    private static ProductVariant?
        GetVariant(
            Product product,
            int? productVariantId)
    {
        var hasVariants =
            product.Variants != null &&
            product.Variants.Any();


        if (hasVariants &&
            !productVariantId.HasValue)
        {
            throw new Exception(
                "Product variant is required.");
        }


        if (!productVariantId.HasValue)
        {
            return null;
        }


        var variant =
            product.Variants
                .FirstOrDefault(
                    x =>
                        x.Id ==
                        productVariantId.Value);


        if (variant == null)
        {
            throw new Exception(
                "Invalid product variant.");
        }


        return variant;
    }


    // DELIVERY

    private static decimal
        CalculateDeliveryFee(
            decimal subtotal)
    {
        return subtotal < 499
            ? 100
            : 0;
    }


    // SHIPPING

    private static void
        ValidateShipping(
            CreateOrderDto dto)
    {
        ValidateShipping(
            dto.Name,
            dto.Address,
            dto.Place,
            dto.State,
            dto.Pincode,
            dto.Phone,
            dto.PaymentMethod);
    }


    private static void
        ValidateShipping(
            string? name,
            string? address,
            string? place,
            string? state,
            string? pincode,
            string? phone,
            string? paymentMethod)
    {
        if (string.IsNullOrWhiteSpace(
                name))
        {
            throw new Exception(
                "Name is required.");
        }


        if (string.IsNullOrWhiteSpace(
                address))
        {
            throw new Exception(
                "Address is required.");
        }


        if (string.IsNullOrWhiteSpace(
                place))
        {
            throw new Exception(
                "Place is required.");
        }


        if (string.IsNullOrWhiteSpace(
                state))
        {
            throw new Exception(
                "State is required.");
        }


        if (string.IsNullOrWhiteSpace(
                pincode))
        {
            throw new Exception(
                "Pincode is required.");
        }


        if (string.IsNullOrWhiteSpace(
                phone))
        {
            throw new Exception(
                "Phone is required.");
        }


        if (string.IsNullOrWhiteSpace(
                paymentMethod))
        {
            throw new Exception(
                "Payment method is required.");
        }
    }


    // RESTORE STOCK

    private static void
        RestoreStock(
            Order order)
    {
        foreach (var item in order.Items)
        {
            if (item.ProductVariant != null)
            {
                item.ProductVariant.Stock +=
                    item.Quantity;
            }
            else if (item.Product != null)
            {
                item.Product.Stock +=
                    item.Quantity;
            }
        }
    }


    // STATUS

    private static OrderStatus
        ParseStatus(
            string status)
    {
        var normalized =
            status
                .Trim()
                .Replace(
                    " ",
                    "")
                .ToLowerInvariant();


        return normalized switch
        {
            "orderplaced" =>
                OrderStatus.OrderPlaced,

            "processing" =>
                OrderStatus.Processing,

            "shipped" =>
                OrderStatus.Shipped,

            "outfordelivery" =>
                OrderStatus.OutForDelivery,

            "delivered" =>
                OrderStatus.Delivered,

            "cancelled" =>
                OrderStatus.Cancelled,

            _ =>
                throw new Exception(
                    "Invalid order status.")
        };
    }


    // USER DTO

    private static OrderResponseDto
        MapToUserDto(
            Order order)
    {
        return new OrderResponseDto
        {
            Id =
                order.Id,

            UserId =
                order.UserId,

            PaymentId =
                order.PaymentId,

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

            Place =
                order.ShippingPlace,

            State =
                order.ShippingState,

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

            Status =
                GetStatusText(
                    order.Status),

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

                                // IMAGE FROM PRODUCT
                                ImageUrl =
                                    item.Product?.ImageUrl,

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


    // ADMIN DTO

    private static AdminOrderResponseDto
        MapToAdminDto(
            Order order)
    {
        return new AdminOrderResponseDto
        {
            Id =
                order.Id,

            UserId =
                order.UserId,

            PaymentId =
                order.PaymentId,

            UserName =
                order.User?.Name
                ?? string.Empty,

            UserEmail =
                order.User?.Email
                ?? string.Empty,

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

            Place =
                order.ShippingPlace,

            State =
                order.ShippingState,

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

            Status =
                GetStatusText(
                    order.Status),

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

                                // IMAGE FROM PRODUCT
                                ImageUrl =
                                    item.Product?.ImageUrl,

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


    // STATUS TEXT

    private static string
        GetStatusText(
            OrderStatus status)
    {
        return status switch
        {
            OrderStatus.OrderPlaced =>
                "Order Placed",

            OrderStatus.Processing =>
                "Processing",

            OrderStatus.Shipped =>
                "Shipped",

            OrderStatus.OutForDelivery =>
                "Out for Delivery",

            OrderStatus.Delivered =>
                "Delivered",

            OrderStatus.Cancelled =>
                "Cancelled",

            _ =>
                "Order Placed"
        };
    }
}