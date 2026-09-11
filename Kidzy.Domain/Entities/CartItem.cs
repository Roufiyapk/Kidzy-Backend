namespace Kidzy.Domain.Entities
{
    public class CartItem
    {
        public int Id { get; set; }

        public int CartId { get; set; }

        public int ProductId { get; set; }

        public string SelectedSize { get; set; } = string.Empty;

        public int Quantity { get; set; }

        // Cart relationship
        public Cart Cart { get; set; } = null!;

        // Product relationship
        public Product Product { get; set; } = null!;
    }
}