namespace Kidzy.Application.DTOs.Orders;

public class CreateOrderDto
{
    public int? PaymentId { get; set; }

    public string Name { get; set; }
        = string.Empty;

    public string Address { get; set; }
        = string.Empty;

    public string Place { get; set; }
        = string.Empty;

    public string State { get; set; }
        = string.Empty;

    public string Pincode { get; set; }
        = string.Empty;

    public string Phone { get; set; }
        = string.Empty;

    public string PaymentMethod { get; set; }
        = string.Empty;
}