namespace Kidzy.Application.DTOs.Products;

public class ProductVariantDto
{
    public string AgeGroup { get; set; } = string.Empty;

    public List<SizeStockDto> Sizes { get; set; }
        = new();
}