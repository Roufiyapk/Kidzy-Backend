namespace Kidzy.Application.DTOs.Checkout
{
    public class BuyNowDto
    {
        public int ProductId { get; set; }

        public string SelectedSize { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = "COD";
    }
}