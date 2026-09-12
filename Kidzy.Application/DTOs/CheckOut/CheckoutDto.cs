namespace Kidzy.Application.DTOs.Checkout
{
    public class CheckoutDto
    {
        public string CustomerName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = "COD";
    }
}