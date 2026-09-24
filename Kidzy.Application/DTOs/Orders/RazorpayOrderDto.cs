namespace Kidzy.Application.DTOs.Orders;

public class RazorpayOrderDto
{
    public string Id { get; set; }
        = string.Empty;

    public long Amount { get; set; }

    public string Currency { get; set; }
        = "INR";

    public string KeyId { get; set; }
        = string.Empty;
}