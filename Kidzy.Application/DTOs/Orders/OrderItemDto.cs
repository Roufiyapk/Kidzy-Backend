namespace Kidzy.Application.DTOs.Orders;

public class OrderItemDto
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int? ProductVariantId { get; set; }

    public string ProductName { get; set; }
        = string.Empty;

    public string? AgeGroup { get; set; }

    public string? Size { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public decimal TotalPrice { get; set; }
}