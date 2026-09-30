namespace Kidzy.Application.DTOs.Products;

public class CreateProductVariantDto
{
    public string AgeGroup { get; set; } = string.Empty;

    public List<SizeStockDto> Sizes { get; set; }
        = new();
}