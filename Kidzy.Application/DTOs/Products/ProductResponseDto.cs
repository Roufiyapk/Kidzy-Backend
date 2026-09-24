namespace Kidzy.Application.DTOs.Products;

public class ProductResponseDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public bool BestSeller { get; set; }

    public bool NewArrival { get; set; }

    public int CategoryId { get; set; }

    public string CategoryName { get; set; }
        = string.Empty;

    public string SubCategory { get; set; }
        = string.Empty;

    public List<ProductVariantDto> Variants { get; set; }
        = new();
}