using Kidzy.Application.DTOs.Orders;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;
using Kidzy.Domain.Entities;
using Kidzy.Domain.Enums;

namespace Kidzy.Application.Services;

public class OrderService
    : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    private readonly IRazorpayService _razorpayService;

    public OrderService(
        IOrderRepository orderRepository,
        IRazorpayService razorpayService)
    {
        _orderRepository =
            orderRepository;

        _razorpayService =
            razorpayService;
    }


    // CART
    
    public async Task<CheckoutResponseDto>
        CreateOrderFromCartAsync(
            int userId,
            CreateOrderDto dto)
    {
        var paymentMethod =
            dto.PaymentMethod
                .Trim()
                .ToLowerInvariant();


        // GET CART

        var cart =
            await _orderRepository
                .GetCartAsync(userId);

        if (cart == null ||
            !cart.Items.Any())
        {
            throw new Exception(
                "Cart is empty.");
        }


        // CHECK STOCK

        foreach (var item in cart.Items)
        {
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
            subtotal + deliveryFee;


        // RAZORPAY FIRST CALL

        if (paymentMethod == "razorpay" &&
            !HasPaymentDetails(dto))
        {
            var receipt =
                CreateReceipt(
                    "KC",
                    userId);

            var razorpayOrder =
                await _razorpayService
                    .CreateOrderAsync(
                        total,
                        receipt);

            return new CheckoutResponseDto
            {
                PaymentRequired =
                    true,

                RazorpayOrder =
                    razorpayOrder,

                Order =
                    null
            };
        }


        // RAZORPAY SECOND CALL

        if (paymentMethod == "razorpay")
        {
            await VerifyRazorpayPayment(
                dto.RazorpayOrderId,
                dto.RazorpayPaymentId,
                dto.RazorpaySignature,
                total,
                $"KC-{userId}-");
        }


        // CREATE ACTUAL ORDER

        var order =
            new Order
            {
                UserId =
                    userId,

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

                ShippingPincode =
                    dto.Pincode.Trim(),

                PaymentMethod =
                    paymentMethod,

                PaymentStatus =
                    paymentMethod == "razorpay"
                        ? "Paid"
                        : "Pending",

                RazorpayPaymentId =
                    paymentMethod == "razorpay"
                        ? dto.RazorpayPaymentId
                        : null,

                RazorpayOrderId =
                    paymentMethod == "razorpay"
                        ? dto.RazorpayOrderId
                        : null,

                RazorpaySignature =
                    paymentMethod == "razorpay"
                        ? dto.RazorpaySignature
                        : null,

                Status =
                    OrderStatus.OrderPlaced,

                CreatedAt =
                    DateTime.UtcNow
            };


        // ORDER ITEMS

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


            // REDUCE STOCK

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
            .AddAsync(order);


        // CLEAR CART

        _orderRepository
            .RemoveCartItems(
                cart.Items);


        await _orderRepository
            .SaveChangesAsync();


        return new CheckoutResponseDto
        {
            PaymentRequired =
                false,

            RazorpayOrder =
                null,

            Order =
                MapToDto(order)
        };
    }


    // BUY NOW
   

    public async Task<CheckoutResponseDto>
        CreateBuyNowOrderAsync(
            int userId,
            BuyNowOrderDto dto)
    {
        var paymentMethod =
            dto.PaymentMethod
                .Trim()
                .ToLowerInvariant();


        // GET PRODUCT

        var product =
            await _orderRepository
                .GetProductWithVariantsAsync(
                    dto.ProductId);

        if (product == null)
        {
            throw new Exception(
                "Product not found.");
        }


        // GET VARIANT

        var variant =
            GetVariant(
                product,
                dto.ProductVariantId);


        // CHECK STOCK

        CheckStock(
            product,
            variant,
            dto.Quantity);


        // CALCULATE TOTAL

        var subtotal =
            product.Price *
            dto.Quantity;

        var deliveryFee =
            CalculateDeliveryFee(
                subtotal);

        var total =
            subtotal + deliveryFee;


        // RAZORPAY FIRST CALL

        if (paymentMethod == "razorpay" &&
            !HasPaymentDetails(dto))
        {
            var receipt =
                CreateReceipt(
                    "KB",
                    userId);

            var razorpayOrder =
                await _razorpayService
                    .CreateOrderAsync(
                        total,
                        receipt);

            return new CheckoutResponseDto
            {
                PaymentRequired =
                    true,

                RazorpayOrder =
                    razorpayOrder,

                Order =
                    null
            };
        }


        // RAZORPAY SECOND CALL

        if (paymentMethod == "razorpay")
        {
            await VerifyRazorpayPayment(
                dto.RazorpayOrderId,
                dto.RazorpayPaymentId,
                dto.RazorpaySignature,
                total,
                $"KB-{userId}-");
        }


        // CREATE ORDER

        var order =
            new Order
            {
                UserId =
                    userId,

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

                ShippingPincode =
                    dto.Pincode.Trim(),

                PaymentMethod =
                    paymentMethod,

                PaymentStatus =
                    paymentMethod == "razorpay"
                        ? "Paid"
                        : "Pending",

                RazorpayPaymentId =
                    paymentMethod == "razorpay"
                        ? dto.RazorpayPaymentId
                        : null,

                RazorpayOrderId =
                    paymentMethod == "razorpay"
                        ? dto.RazorpayOrderId
                        : null,

                RazorpaySignature =
                    paymentMethod == "razorpay"
                        ? dto.RazorpaySignature
                        : null,

                Status =
                    OrderStatus.OrderPlaced,

                CreatedAt =
                    DateTime.UtcNow
            };


        // ORDER ITEM

        var orderItem =
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
            };

        order.Items.Add(
            orderItem);


        // STOCK REDUCTION

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
            .AddAsync(order);


        // Buy Now → cart untouched

        await _orderRepository
            .SaveChangesAsync();


        return new CheckoutResponseDto
        {
            PaymentRequired =
                false,

            RazorpayOrder =
                null,

            Order =
                MapToDto(order)
        };
    }


    // GET USER ORDERS

    public async Task<List<OrderResponseDto>>
        GetUserOrdersAsync(
            int userId)
    {
        var orders =
            await _orderRepository
                .GetUserOrdersAsync(
                    userId);

        return orders
            .Select(MapToDto)
            .ToList();
    }


    // GET SINGLE ORDER

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


    // CANCEL ORDER

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


        if (order.Status !=
                OrderStatus.OrderPlaced &&
            order.Status !=
                OrderStatus.Processing)
        {
            throw new Exception(
                "This order cannot be cancelled.");
        }


        // RESTOCK

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


        order.Status =
            OrderStatus.Cancelled;

        order.CancelledAt =
            DateTime.UtcNow;


        await _orderRepository
            .SaveChangesAsync();


        return MapToDto(order);
    }


    // CHECK RAZORPAY DETAILS

    private static bool HasPaymentDetails(
        CreateOrderDto dto)
    {
        var hasOrderId =
            !string.IsNullOrWhiteSpace(
                dto.RazorpayOrderId);

        var hasPaymentId =
            !string.IsNullOrWhiteSpace(
                dto.RazorpayPaymentId);

        var hasSignature =
            !string.IsNullOrWhiteSpace(
                dto.RazorpaySignature);


        if (!hasOrderId &&
            !hasPaymentId &&
            !hasSignature)
        {
            return false;
        }


        if (!hasOrderId ||
            !hasPaymentId ||
            !hasSignature)
        {
            throw new Exception(
                "Razorpay payment details are incomplete.");
        }


        return true;
    }


    private static bool HasPaymentDetails(
        BuyNowOrderDto dto)
    {
        var hasOrderId =
            !string.IsNullOrWhiteSpace(
                dto.RazorpayOrderId);

        var hasPaymentId =
            !string.IsNullOrWhiteSpace(
                dto.RazorpayPaymentId);

        var hasSignature =
            !string.IsNullOrWhiteSpace(
                dto.RazorpaySignature);


        if (!hasOrderId &&
            !hasPaymentId &&
            !hasSignature)
        {
            return false;
        }


        if (!hasOrderId ||
            !hasPaymentId ||
            !hasSignature)
        {
            throw new Exception(
                "Razorpay payment details are incomplete.");
        }


        return true;
    }


    // VERIFY RAZORPAY

    private async Task
        VerifyRazorpayPayment(
            string? razorpayOrderId,
            string? razorpayPaymentId,
            string? razorpaySignature,
            decimal expectedAmount,
            string expectedReceiptPrefix)
    {
        if (string.IsNullOrWhiteSpace(
                razorpayOrderId) ||
            string.IsNullOrWhiteSpace(
                razorpayPaymentId) ||
            string.IsNullOrWhiteSpace(
                razorpaySignature))
        {
            throw new Exception(
                "Razorpay payment details are required.");
        }


        var alreadyUsed =
            await _orderRepository
                .IsPaymentAlreadyUsedAsync(
                    razorpayPaymentId);

        if (alreadyUsed)
        {
            throw new Exception(
                "This payment has already been used.");
        }


        var valid =
            await _razorpayService
                .VerifyPaymentAsync(
                    razorpayOrderId,
                    razorpayPaymentId,
                    razorpaySignature,
                    expectedAmount,
                    expectedReceiptPrefix);

        if (!valid)
        {
            throw new Exception(
                "Razorpay payment verification failed.");
        }
    }


    // GET VARIANT

    private static ProductVariant?
        GetVariant(
            Product product,
            int? productVariantId)
    {
        var hasVariants =
            product.Variants != null &&
            product.Variants.Any();


        if (hasVariants &&
            productVariantId == null)
        {
            throw new Exception(
                "Please select age group and size.");
        }


        if (!productVariantId.HasValue)
        {
            return null;
        }


        var variant =
            product.Variants
                .FirstOrDefault(
                    v =>
                        v.Id ==
                        productVariantId.Value);


        if (variant == null)
        {
            throw new Exception(
                "Invalid product variant.");
        }


        return variant;
    }


    // STOCK

    private static void CheckStock(
        Product product,
        ProductVariant? variant,
        int quantity)
    {
        if (quantity <= 0)
        {
            throw new Exception(
                "Quantity must be greater than zero.");
        }


        // Product with variant

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
                    $"{product.Name} " +
                    $"({variant.Size}).");
            }


            return;
        }


        // Product without variant

        if (product.Stock <= 0)
        {
            throw new Exception(
                $"{product.Name} is out of stock.");
        }


        if (quantity > product.Stock)
        {
            throw new Exception(
                $"Only {product.Stock} available for " +
                $"{product.Name}.");
        }
    }


    // DELIVERY FEE

    private static decimal
        CalculateDeliveryFee(
            decimal subtotal)
    {
        return subtotal < 499
            ? 100
            : 0;
    }


    // RECEIPT

    private static string
        CreateReceipt(
            string type,
            int userId)
    {
        var randomPart =
            Guid.NewGuid()
                .ToString("N")
                .Substring(0, 16);

        return $"{type}-{userId}-{randomPart}";
    }


    // =====================================================
    // MAP ORDER
    // =====================================================

    private static OrderResponseDto
        MapToDto(
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