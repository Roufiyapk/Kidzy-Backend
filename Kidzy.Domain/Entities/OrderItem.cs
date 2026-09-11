namespace Kidzy.Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        public string Image { get; set; } = string.Empty;

        public string SelectedSize { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal SubTotal { get; set; }


        public Order Order { get; set; } = null!;

        public Product Product { get; set; } = null!;
    }
}