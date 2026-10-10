namespace Kidzy.Application.DTOs.Products;

public class ProductPagedResponseDto
{
    public List<ProductResponseDto> Products { get; set; }
        = new();

    public int CurrentPage { get; set; }

    public int PageSize { get; set; }

    public int TotalProducts { get; set; }

    public int TotalPages { get; set; }
}