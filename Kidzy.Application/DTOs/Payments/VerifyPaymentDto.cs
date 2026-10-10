namespace Kidzy.Application.DTOs.Payments;

public class VerifyPaymentDto
{
    public string RazorpayOrderId { get; set; }
        = string.Empty;

    public string RazorpayPaymentId { get; set; }
        = string.Empty;

    public string RazorpaySignature { get; set; }
        = string.Empty;
}