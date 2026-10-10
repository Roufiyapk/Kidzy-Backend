using Kidzy.Application.DTOs.Payments;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;

using Kidzy.Domain.Entities;

using Microsoft.Extensions.Configuration;

namespace Kidzy.Application.Services;

public class PaymentService
    : IPaymentService
{
    private readonly IPaymentRepository
        _paymentRepository;

    private readonly IOrderRepository
        _orderRepository;

    private readonly IRazorpayService
        _razorpayService;

    private readonly IConfiguration
        _configuration;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        IRazorpayService razorpayService,
        IConfiguration configuration)
    {
        _paymentRepository =
            paymentRepository;

        _orderRepository =
            orderRepository;

        _razorpayService =
            razorpayService;

        _configuration =
            configuration;
    }

    public async Task<PaymentResponseDto>
        CreateOrderAsync(
            int userId,
            CreatePaymentOrderDto dto)
    {
        if (dto == null)
        {
            throw new Exception(
                "Payment details are required.");
        }

        var checkoutType =
            dto.CheckoutType
                .Trim()
                .ToLowerInvariant();

        if (
            checkoutType != "cart" &&
            checkoutType != "buy-now")
        {
            throw new Exception(
                "CheckoutType must be cart or buy-now.");
        }

        decimal subtotal;

        int? productId = null;

        int? productVariantId = null;

        int quantity =
            dto.Quantity > 0
                ? dto.Quantity
                : 1;

        if (checkoutType == "cart")
        {
            var cart =
                await _orderRepository
                    .GetCartAsync(userId);

            if (
                cart == null ||
                cart.Items == null ||
                !cart.Items.Any())
            {
                throw new Exception(
                    "Cart is empty.");
            }

            foreach (
                var cartItem
                in cart.Items)
            {
                if (
                    cartItem.Product ==
                    null)
                {
                    throw new Exception(
                        "Product not found in cart.");
                }

                if (
                    cartItem.ProductVariantId
                        .HasValue &&
                    cartItem.ProductVariant
                        == null)
                {
                    throw new Exception(
                        "Product variant not found in cart.");
                }

                if (
                    cartItem.ProductVariant
                    != null)
                {
                    if (
                        cartItem.ProductVariant.Stock
                        <= 0)
                    {
                        throw new Exception(
                            $"{cartItem.Product.Name} - {cartItem.ProductVariant.Size} is out of stock.");
                    }

                    if (
                        cartItem.Quantity >
                        cartItem.ProductVariant.Stock)
                    {
                        throw new Exception(
                            $"Only {cartItem.ProductVariant.Stock} available for {cartItem.Product.Name} ({cartItem.ProductVariant.Size}).");
                    }
                }
                else
                {
                    if (
                        cartItem.Product.Stock
                        <= 0)
                    {
                        throw new Exception(
                            $"{cartItem.Product.Name} is out of stock.");
                    }

                    if (
                        cartItem.Quantity >
                        cartItem.Product.Stock)
                    {
                        throw new Exception(
                            $"Only {cartItem.Product.Stock} available for {cartItem.Product.Name}.");
                    }
                }
            }

            subtotal =
                cart.Items.Sum(
                    item =>
                        item.Product.Price *
                        item.Quantity);
        }
        else
        {
            if (
                !dto.ProductId.HasValue ||
                dto.ProductId.Value <= 0)
            {
                throw new Exception(
                    "ProductId is required.");
            }

            productId =
                dto.ProductId.Value;

            productVariantId =
                dto.ProductVariantId;

            var product =
                await _orderRepository
                    .GetProductWithVariantsAsync(
                        productId.Value);

            if (product == null)
            {
                throw new Exception(
                    "Product not found.");
            }

            ProductVariant? variant =
                null;

            if (
                productVariantId.HasValue)
            {
                variant =
                    product.Variants
                        .FirstOrDefault(
                            x =>
                                x.Id ==
                                productVariantId.Value);

                if (variant == null)
                {
                    throw new Exception(
                        "Product variant not found.");
                }

                if (
                    variant.ProductId !=
                    product.Id)
                {
                    throw new Exception(
                        "Product variant does not belong to this product.");
                }

                if (variant.Stock <= 0)
                {
                    throw new Exception(
                        $"{product.Name} - {variant.Size} is out of stock.");
                }

                if (
                    quantity >
                    variant.Stock)
                {
                    throw new Exception(
                        $"Only {variant.Stock} available for {product.Name} ({variant.Size}).");
                }
            }
            else
            {
                if (product.Stock <= 0)
                {
                    throw new Exception(
                        $"{product.Name} is out of stock.");
                }

                if (
                    quantity >
                    product.Stock)
                {
                    throw new Exception(
                        $"Only {product.Stock} available for {product.Name}.");
                }
            }

            subtotal =
                product.Price *
                quantity;
        }

        var deliveryFee =
            subtotal < 499
                ? 100m
                : 0m;

        var total =
            subtotal +
            deliveryFee;

        var receipt =
            CreateReceipt(
                checkoutType,
                userId);

        var razorpayOrder =
            await _razorpayService
                .CreateOrderAsync(
                    total,
                    receipt);

        var keyId =
            _configuration[
                "Razorpay:KeyId"];

        if (
            string.IsNullOrWhiteSpace(
                keyId))
        {
            throw new Exception(
                "Razorpay KeyId is missing in appsettings.json.");
        }

        var payment =
            new Payment
            {
                UserId =
                    userId,

                RazorpayOrderId =
                    razorpayOrder.Id,

                RazorpayPaymentId =
                    null,

                RazorpaySignature =
                    null,

                Amount =
                    total,

                Currency =
                    razorpayOrder.Currency,

                Receipt =
                    receipt,

                Status =
                    "Created",

                CheckoutType =
                    checkoutType,

                ProductId =
                    productId,

                ProductVariantId =
                    productVariantId,

                Quantity =
                    quantity,

                CreatedAt =
                    DateTime.UtcNow,

                VerifiedAt =
                    null
            };

        await _paymentRepository
            .AddAsync(payment);

        await _paymentRepository
            .SaveChangesAsync();

        return new PaymentResponseDto
        {
            PaymentId =
                payment.Id,

            Success =
                true,

            Status =
                payment.Status,

            Amount =
                payment.Amount,

            Currency =
                payment.Currency,

            RazorpayOrderId =
                payment.RazorpayOrderId,

            RazorpayPaymentId =
                null,

            KeyId =
                keyId,

            RazorpayOrder =
                razorpayOrder,

            Message =
                "Razorpay order created successfully."
        };
    }

    public async Task<PaymentResponseDto>
        VerifyPaymentAsync(
            int userId,
            VerifyPaymentDto dto)
    {
        if (dto == null)
        {
            throw new Exception(
                "Payment verification details are required.");
        }

        if (
            string.IsNullOrWhiteSpace(
                dto.RazorpayOrderId))
        {
            throw new Exception(
                "RazorpayOrderId is required.");
        }

        if (
            string.IsNullOrWhiteSpace(
                dto.RazorpayPaymentId))
        {
            throw new Exception(
                "RazorpayPaymentId is required.");
        }

        if (
            string.IsNullOrWhiteSpace(
                dto.RazorpaySignature))
        {
            throw new Exception(
                "RazorpaySignature is required.");
        }

        var payment =
            await _paymentRepository
                .GetByRazorpayOrderIdAsync(
                    dto.RazorpayOrderId);

        if (payment == null)
        {
            throw new Exception(
                "Payment record not found.");
        }

        if (
            payment.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to verify this payment.");
        }

        if (
            payment.Status == "Paid")
        {
            throw new Exception(
                "Payment has already been verified.");
        }

        if (
            await _paymentRepository
                .IsPaymentAlreadyUsedAsync(
                    dto.RazorpayPaymentId))
        {
            throw new Exception(
                "This Razorpay payment has already been used.");
        }

        var isValid =
            await _razorpayService
                .VerifyPaymentAsync(
                    dto.RazorpayOrderId,
                    dto.RazorpayPaymentId,
                    dto.RazorpaySignature);

        if (!isValid)
        {
            payment.Status =
                "Failed";

            await _paymentRepository
                .SaveChangesAsync();

            throw new Exception(
                "Payment verification failed.");
        }

        payment.RazorpayPaymentId =
            dto.RazorpayPaymentId;

        payment.RazorpaySignature =
            dto.RazorpaySignature;

        payment.Status =
            "Paid";

        payment.VerifiedAt =
            DateTime.UtcNow;

        await _paymentRepository
            .SaveChangesAsync();

        var keyId =
            _configuration[
                "Razorpay:KeyId"]
            ?? string.Empty;

        return new PaymentResponseDto
        {
            PaymentId =
                payment.Id,

            Success =
                true,

            Status =
                payment.Status,

            Amount =
                payment.Amount,

            Currency =
                payment.Currency,

            RazorpayOrderId =
                payment.RazorpayOrderId,

            RazorpayPaymentId =
                payment.RazorpayPaymentId,

            KeyId =
                keyId,

            RazorpayOrder =
                null,

            Message =
                "Payment verified successfully."
        };
    }

    public async Task<PaymentResponseDto?>
        GetPaymentAsync(
            int userId,
            int paymentId)
    {
        var payment =
            await _paymentRepository
                .GetByIdAsync(
                    paymentId);

        if (payment == null)
        {
            return null;
        }

        if (
            payment.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to view this payment.");
        }

        var keyId =
            _configuration[
                "Razorpay:KeyId"]
            ?? string.Empty;

        return new PaymentResponseDto
        {
            PaymentId =
                payment.Id,

            Success =
                payment.Status == "Paid",

            Status =
                payment.Status,

            Amount =
                payment.Amount,

            Currency =
                payment.Currency,

            RazorpayOrderId =
                payment.RazorpayOrderId,

            RazorpayPaymentId =
                payment.RazorpayPaymentId,

            KeyId =
                keyId,

            Message =
                "Payment retrieved successfully."
        };
    }

    private static string
        CreateReceipt(
            string checkoutType,
            int userId)
    {
        var prefix =
            checkoutType == "buy-now"
                ? "KB"
                : "KC";

        var random =
            Guid.NewGuid()
                .ToString("N")
                .Substring(
                    0,
                    16);

        return
            $"{prefix}-{userId}-{random}";
    }
}