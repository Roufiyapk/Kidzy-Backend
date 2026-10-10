namespace Kidzy.Application.DTOs.Payments;

public class PaymentResponseDto
{
    public int PaymentId { get; set; }

    public bool Success { get; set; }

    public string Status { get; set; }
        = string.Empty;

    public decimal Amount { get; set; }

    public string Currency { get; set; }
        = "INR";

    public string RazorpayOrderId { get; set; }
        = string.Empty;

    public string? RazorpayPaymentId { get; set; }

    public string KeyId { get; set; }
        = string.Empty;

    public RazorpayOrderResponseDto? RazorpayOrder { get; set; }

    public string Message { get; set; }
        = string.Empty;
}