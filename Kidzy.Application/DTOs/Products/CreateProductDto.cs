using Microsoft.AspNetCore.Http;

namespace Kidzy.Application.DTOs.Products;

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public IFormFile? Image { get; set; }

    public bool BestSeller { get; set; }

    public bool NewArrival { get; set; }

    public int CategoryId { get; set; }

    public int SubCategoryId { get; set; }

    public List<CreateProductVariantDto> Variants { get; set; }
        = new();
}