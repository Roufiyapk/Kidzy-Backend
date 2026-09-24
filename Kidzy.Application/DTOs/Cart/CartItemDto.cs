namespace Kidzy.Application.DTOs.Cart;

public class CartItemDto
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int? ProductVariantId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public string? AgeGroup { get; set; }

    public string? Size { get; set; }

    public int Quantity { get; set; }

    public decimal TotalPrice { get; set; }
}