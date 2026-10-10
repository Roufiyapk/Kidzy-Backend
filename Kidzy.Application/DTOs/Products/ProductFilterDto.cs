namespace Kidzy.Application.DTOs.Products;

public class ProductFilterDto
{
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }

    public int? SubCategoryId { get; set; }
    public string? SubCategoryName { get; set; }

    public string? Age { get; set; }
    public string? Gender { get; set; }
    public string? PriceRange { get; set; }
    public string? StockStatus { get; set; }

    public bool? BestSeller { get; set; }
    public bool? NewArrival { get; set; }

    public string? SortBy { get; set; }

    public string? Search { get; set; }
    public string? Status { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 5;
}