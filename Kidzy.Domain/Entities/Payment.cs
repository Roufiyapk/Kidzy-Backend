namespace Kidzy.Domain.Entities;

public class Payment
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = null!;


    public string RazorpayOrderId { get; set; }
        = string.Empty;

    public string? RazorpayPaymentId { get; set; }

    public string? RazorpaySignature { get; set; }


    public decimal Amount { get; set; }

    public string Currency { get; set; }
        = "INR";

    public string Receipt { get; set; }
        = string.Empty;

    public string Status { get; set; }
        = "Created";


    public string CheckoutType { get; set; }
        = string.Empty;


    public int? ProductId { get; set; }

    public int? ProductVariantId { get; set; }

    public int Quantity { get; set; }


    public int? OrderId { get; set; }

    public Order? Order { get; set; }


    public DateTime CreatedAt { get; set; }
        = DateTime.UtcNow;

    public DateTime? VerifiedAt { get; set; }
}