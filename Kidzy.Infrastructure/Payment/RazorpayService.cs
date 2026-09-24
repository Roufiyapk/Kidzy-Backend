using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kidzy.Application.DTOs.Orders;
using Kidzy.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace Kidzy.Infrastructure.Payment;

public class RazorpayService
    : IRazorpayService
{
    private readonly HttpClient _httpClient;

    private readonly string _keyId;

    private readonly string _keySecret;

    public RazorpayService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;

        _keyId =
            configuration["Razorpay:KeyId"]
            ?? string.Empty;

        _keySecret =
            configuration["Razorpay:KeySecret"]
            ?? string.Empty;

        if (string.IsNullOrWhiteSpace(
                _keyId))
        {
            throw new Exception(
                "Razorpay KeyId is not configured.");
        }

        if (string.IsNullOrWhiteSpace(
                _keySecret))
        {
            throw new Exception(
                "Razorpay KeySecret is not configured.");
        }

        _httpClient.BaseAddress =
            new Uri(
                "https://api.razorpay.com/v1/");
    }


    // ==========================================
    // CREATE RAZORPAY ORDER
    // ==========================================

    public async Task<RazorpayOrderDto>
        CreateOrderAsync(
            decimal amount,
            string receipt)
    {
        var amountInPaise =
            Convert.ToInt64(
                Math.Round(
                    amount * 100,
                    MidpointRounding.AwayFromZero));

        var request =
            new HttpRequestMessage(
                HttpMethod.Post,
                "orders");

        AddAuthorization(request);

        var body =
            new
            {
                amount =
                    amountInPaise,

                currency =
                    "INR",

                receipt =
                    receipt,

                partial_payment =
                    false
            };

        request.Content =
            new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json");

        var response =
            await _httpClient
                .SendAsync(request);

        var responseBody =
            await response.Content
                .ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Razorpay order creation failed: " +
                $"{responseBody}");
        }

        var razorpayOrder =
            JsonSerializer.Deserialize
                <RazorpayOrderResponse>(
                    responseBody);

        if (razorpayOrder == null ||
            string.IsNullOrWhiteSpace(
                razorpayOrder.Id))
        {
            throw new Exception(
                "Invalid Razorpay order response.");
        }

        return new RazorpayOrderDto
        {
            Id =
                razorpayOrder.Id,

            Amount =
                razorpayOrder.Amount,

            Currency =
                razorpayOrder.Currency
                ?? "INR",

            KeyId =
                _keyId
        };
    }


    // VERIFY PAYMENT

    public async Task<bool>
        VerifyPaymentAsync(
            string razorpayOrderId,
            string razorpayPaymentId,
            string razorpaySignature,
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
            return false;
        }


        var razorpayOrder =
            await GetOrderAsync(
                razorpayOrderId);

        if (razorpayOrder == null)
            return false;


        if (string.IsNullOrWhiteSpace(
                razorpayOrder.Receipt))
        {
            return false;
        }


        // Check order belongs to
        // this checkout/user prefix.

        if (!razorpayOrder.Receipt
                .StartsWith(
                    expectedReceiptPrefix,
                    StringComparison.Ordinal))
        {
            return false;
        }


        var expectedAmountInPaise =
            Convert.ToInt64(
                Math.Round(
                    expectedAmount * 100,
                    MidpointRounding.AwayFromZero));


        if (razorpayOrder.Amount !=
            expectedAmountInPaise)
        {
            return false;
        }


        var payment =
            await GetPaymentAsync(
                razorpayPaymentId);

        if (payment == null)
            return false;


        if (!string.Equals(
                payment.OrderId,
                razorpayOrder.Id,
                StringComparison.Ordinal))
        {
            return false;
        }


        if (payment.Amount !=
            expectedAmountInPaise)
        {
            return false;
        }


        if (!string.Equals(
                payment.Status,
                "captured",
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }


        return VerifySignature(
            razorpayOrder.Id,
            razorpayPaymentId,
            razorpaySignature);
    }


    // SIGNATURE VERIFICATION

    private bool VerifySignature(
        string orderId,
        string paymentId,
        string signature)
    {
        using var hmac =
            new HMACSHA256(
                Encoding.UTF8.GetBytes(
                    _keySecret));

        var payload =
            $"{orderId}|{paymentId}";

        var hash =
            hmac.ComputeHash(
                Encoding.UTF8.GetBytes(
                    payload));

        var expectedSignature =
            Convert.ToHexString(hash)
                .ToLowerInvariant();

        var actualSignature =
            signature
                .Trim()
                .ToLowerInvariant();

        return CryptographicOperations
            .FixedTimeEquals(
                Encoding.UTF8.GetBytes(
                    expectedSignature),

                Encoding.UTF8.GetBytes(
                    actualSignature));
    }


    // GET RAZORPAY ORDER

    private async Task<RazorpayOrderResponse?>
        GetOrderAsync(
            string orderId)
    {
        var request =
            new HttpRequestMessage(
                HttpMethod.Get,
                $"orders/{orderId}");

        AddAuthorization(request);

        var response =
            await _httpClient
                .SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return null;

        var body =
            await response.Content
                .ReadAsStringAsync();

        return JsonSerializer.Deserialize
            <RazorpayOrderResponse>(
                body);
    }


    // GET PAYMENT

    private async Task<RazorpayPaymentResponse?>
        GetPaymentAsync(
            string paymentId)
    {
        var request =
            new HttpRequestMessage(
                HttpMethod.Get,
                $"payments/{paymentId}");

        AddAuthorization(request);

        var response =
            await _httpClient
                .SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return null;

        var body =
            await response.Content
                .ReadAsStringAsync();

        return JsonSerializer.Deserialize
            <RazorpayPaymentResponse>(
                body);
    }


    // BASIC AUTH

    private void AddAuthorization(
        HttpRequestMessage request)
    {
        var credentials =
            Convert.ToBase64String(
                Encoding.UTF8.GetBytes(
                    $"{_keyId}:{_keySecret}"));

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Basic",
                credentials);
    }


    // ORDER RESPONSE

    private sealed class RazorpayOrderResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
            = string.Empty;

        [JsonPropertyName("amount")]
        public long Amount { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("receipt")]
        public string? Receipt { get; set; }
    }


    // PAYMENT RESPONSE

    private sealed class RazorpayPaymentResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
            = string.Empty;

        [JsonPropertyName("order_id")]
        public string? OrderId { get; set; }

        [JsonPropertyName("amount")]
        public long Amount { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }
}