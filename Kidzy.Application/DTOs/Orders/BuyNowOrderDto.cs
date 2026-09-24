namespace Kidzy.Application.DTOs.Orders;

public class BuyNowOrderDto
{
    public int ProductId { get; set; }

    public int? ProductVariantId { get; set; }

    public int Quantity { get; set; } = 1;

    public string Name { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string Pincode { get; set; } = string.Empty;

    public string PaymentMethod { get; set; } = string.Empty;

    public string? RazorpayPaymentId { get; set; }

    public string? RazorpayOrderId { get; set; }

    public string? RazorpaySignature { get; set; }
}