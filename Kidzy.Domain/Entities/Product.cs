using System.Collections.Generic;

namespace Kidzy.Domain.Entities;

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public bool BestSeller { get; set; }

    public bool NewArrival { get; set; }

    public string? Gender { get; set; }

    public bool Deleted { get; set; } = false;

    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public int? SubCategoryId { get; set; }

    public SubCategory? SubCategory { get; set; }

    public ICollection<ProductVariant> Variants { get; set; }
        = new List<ProductVariant>();
}