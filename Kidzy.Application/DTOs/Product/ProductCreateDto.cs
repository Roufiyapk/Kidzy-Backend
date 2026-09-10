namespace Kidzy.Application.DTOs.Product
{
    public class ProductCreateDto
    {
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        public string Age { get; set; } = string.Empty;

        public string Image { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool BestSeller { get; set; }

        public bool NewArrival { get; set; }

        public List<ProductSizeDto> Sizes { get; set; }
            = new List<ProductSizeDto>();
    }
}