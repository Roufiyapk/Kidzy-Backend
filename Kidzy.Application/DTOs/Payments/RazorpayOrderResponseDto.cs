namespace Kidzy.Application.DTOs.Payments;

public class RazorpayOrderResponseDto
{
    public string Id { get; set; } = string.Empty;

    public string KeyId { get; set; } = string.Empty;

    public long Amount { get; set; }

    public string Currency { get; set; } = "INR";

    public string Receipt { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
}