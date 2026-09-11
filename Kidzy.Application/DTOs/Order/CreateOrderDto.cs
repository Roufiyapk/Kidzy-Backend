namespace Kidzy.Application.DTOs.Order
{
    public class CreateOrderDto
    {
        public string CustomerName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = string.Empty;
    }
}