namespace Kidzy.Application.DTOs.Cart
{
    public class AddToCartDto
    {
        public int ProductId { get; set; }

        public string SelectedSize { get; set; } = string.Empty;

        public int Quantity { get; set; }
    }
}