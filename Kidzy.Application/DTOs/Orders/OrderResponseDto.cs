namespace Kidzy.Application.DTOs.Orders;

public class OrderResponseDto
{
    public int Id { get; set; }

    public decimal Subtotal { get; set; }

    public decimal DeliveryFee { get; set; }

    public decimal TotalAmount { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string Pincode { get; set; } = string.Empty;

    public string PaymentMethod { get; set; } = string.Empty;

    public string PaymentStatus { get; set; } = string.Empty;

    public string? RazorpayPaymentId { get; set; }

    public string? RazorpayOrderId { get; set; }

    public string? RazorpaySignature { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public List<OrderItemDto> Items { get; set; }
        = new();
}