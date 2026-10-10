namespace Kidzy.Application.DTOs.Products;

public class ProductResponseDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public List<string> Images { get; set; } = new();

    public bool BestSeller { get; set; }

    public bool NewArrival { get; set; }

    public string? Gender { get; set; }

    public bool Deleted { get; set; }

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public int? SubCategoryId { get; set; }
    public string SubCategory { get; set; } = string.Empty;

    public List<ProductVariantDto> Variants { get; set; } = new();
}
