namespace Kidzy.Application.DTOs.Products;

public class ProductFilterDto
{
    // Subcategory filter
    public int? SubCategoryId { get; set; }

    // Age filter
    public string? Age { get; set; }

    // Gender filter
    public string? Gender { get; set; }

    // Price range filter
    public string? PriceRange { get; set; }

    // Sorting
    public string? SortBy { get; set; }
}