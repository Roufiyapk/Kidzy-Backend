namespace Kidzy.Application.DTOs.Orders;

public class CheckoutResponseDto
{
    public bool PaymentRequired { get; set; }

    public object? RazorpayOrder { get; set; }

    public OrderResponseDto? Order { get; set; }
}