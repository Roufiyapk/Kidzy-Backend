namespace Kidzy.Domain.Entities;

public class CartItem
{
    public int Id { get; set; }

    public int CartId { get; set; }
    public Cart Cart { get; set; } = null!;

    // Product
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    // Selected size/variant
    public int? ProductVariantId { get; set; }
    public ProductVariant? ProductVariant { get; set; }

    // Quantity selected by user
    public int Quantity { get; set; }
}