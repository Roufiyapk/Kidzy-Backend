using Microsoft.AspNetCore.Http;

namespace Kidzy.Application.DTOs.Products;

public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public int CategoryId { get; set; }

    public int? SubCategoryId { get; set; }
    public string? Gender { get; set; }

    public bool BestSeller { get; set; }

    public bool NewArrival { get; set; }

    public List<IFormFile> Images { get; set; } = new();

    public List<CreateProductVariantDto> Variants { get; set; } = new();
}