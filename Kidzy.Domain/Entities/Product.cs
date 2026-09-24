namespace Kidzy.Domain.Entities;

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public bool BestSeller { get; set; }

    public bool NewArrival { get; set; }

    // Category
    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    // SubCategory
    public int SubCategoryId { get; set; }

    public SubCategory SubCategory { get; set; } = null!;

    // Variants
    public ICollection<ProductVariant> Variants { get; set; }
        = new List<ProductVariant>();
}