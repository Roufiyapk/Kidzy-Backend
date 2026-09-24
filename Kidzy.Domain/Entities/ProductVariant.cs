namespace Kidzy.Domain.Entities;

public class ProductVariant
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public string? AgeGroup { get; set; }

    public string? Size { get; set; }

    public int Stock { get; set; }
}