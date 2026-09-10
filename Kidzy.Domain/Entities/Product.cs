namespace Kidzy.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        public string Age { get; set; } = string.Empty;

        public string Image { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool BestSeller { get; set; }

        public bool NewArrival { get; set; }

        // Category relationship
        public Category Category { get; set; } = null!;

        // Product → ProductSizes
        public ICollection<ProductSize> ProductSizes { get; set; }
            = new List<ProductSize>();
    }
}