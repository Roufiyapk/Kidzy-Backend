using System.Security.Cryptography;
using System.Text;

using Kidzy.Application.DTOs.Payments;
using Kidzy.Application.Interfaces.Services;

using Microsoft.Extensions.Configuration;

using Razorpay.Api;

namespace Kidzy.Infrastructure.Payment;

public class RazorpayService : IRazorpayService
{
    private readonly string _keyId;
    private readonly string _keySecret;

    public RazorpayService(
        IConfiguration configuration)
    {
        _keyId =
            configuration["Razorpay:KeyId"]
            ?? throw new InvalidOperationException(
                "Razorpay KeyId is missing.");

        _keySecret =
            configuration["Razorpay:KeySecret"]
            ?? throw new InvalidOperationException(
                "Razorpay KeySecret is missing.");
    }

    public Task<RazorpayOrderResponseDto> CreateOrderAsync(
        decimal amount,
        string receipt)
    {
        if (amount <= 0)
        {
            throw new Exception(
                "Payment amount must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(receipt))
        {
            throw new Exception(
                "Payment receipt is required.");
        }

        var client =
            new RazorpayClient(
                _keyId,
                _keySecret);

        var amountInPaise =
            checked(
                (long)Math.Round(
                    amount * 100m,
                    MidpointRounding.AwayFromZero));

        var options =
            new Dictionary<string, object>
            {
                ["amount"] = amountInPaise,
                ["currency"] = "INR",
                ["receipt"] = receipt
            };

        var razorpayOrder =
            client.Order.Create(options);

        var result =
            new RazorpayOrderResponseDto
            {
                // Razorpay Order ID
                Id =
                    Convert.ToString(
                        razorpayOrder["id"])
                    ?? string.Empty,

                // Razorpay Public Key ID
                KeyId =
                    _keyId,

                // Amount in paise
                Amount =
                    Convert.ToInt64(
                        razorpayOrder["amount"]),

                Currency =
                    Convert.ToString(
                        razorpayOrder["currency"])
                    ?? "INR",

                Receipt =
                    Convert.ToString(
                        razorpayOrder["receipt"])
                    ?? string.Empty,

                Status =
                    Convert.ToString(
                        razorpayOrder["status"])
                    ?? string.Empty
            };

        return Task.FromResult(result);
    }

    public Task<bool> VerifyPaymentAsync(
        string razorpayOrderId,
        string razorpayPaymentId,
        string razorpaySignature)
    {
        if (string.IsNullOrWhiteSpace(razorpayOrderId))
        {
            throw new Exception(
                "Razorpay order ID is required.");
        }

        if (string.IsNullOrWhiteSpace(razorpayPaymentId))
        {
            throw new Exception(
                "Razorpay payment ID is required.");
        }

        if (string.IsNullOrWhiteSpace(razorpaySignature))
        {
            throw new Exception(
                "Razorpay signature is required.");
        }

        var client =
            new RazorpayClient(
                _keyId,
                _keySecret);

        var razorpayOrder =
            client.Order.Fetch(
                razorpayOrderId);

        var fetchedOrderId =
            Convert.ToString(
                razorpayOrder["id"])
            ?? string.Empty;

        if (!string.Equals(
                fetchedOrderId,
                razorpayOrderId,
                StringComparison.Ordinal))
        {
            return Task.FromResult(false);
        }

        var payload =
            $"{razorpayOrderId}|{razorpayPaymentId}";

        using var hmac =
            new HMACSHA256(
                Encoding.UTF8.GetBytes(
                    _keySecret));

        var hash =
            hmac.ComputeHash(
                Encoding.UTF8.GetBytes(
                    payload));

        var generatedSignature =
            Convert.ToHexString(hash)
                .ToLowerInvariant();

        var suppliedSignature =
            razorpaySignature
                .Trim()
                .ToLowerInvariant();

        var valid =
            CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(
                    generatedSignature),
                Encoding.UTF8.GetBytes(
                    suppliedSignature));

        return Task.FromResult(valid);
    }
}