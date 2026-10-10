namespace Kidzy.Application.DTOs.Payments;

public class CreatePaymentOrderDto
{
    public string CheckoutType { get; set; }
        = "cart";


    public int? ProductId { get; set; }

    public int? ProductVariantId { get; set; }

    public int Quantity { get; set; } = 1;
}