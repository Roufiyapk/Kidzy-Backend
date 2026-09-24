namespace Kidzy.Domain.Entities;

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    // Used for products without variants
    public int Stock { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public bool BestSeller { get; set; }

    public bool NewArrival { get; set; }

    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public int SubCategoryId { get; set; }

    public SubCategory SubCategory { get; set; } = null!;

    public ICollection<ProductVariant> Variants { get; set; }
        = new List<ProductVariant>();
}