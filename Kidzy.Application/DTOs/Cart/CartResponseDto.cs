namespace Kidzy.Application.DTOs.Cart;

public class CartResponseDto
{
    public int CartId { get; set; }

    public List<CartItemDto> Items { get; set; }
        = new();

    public decimal TotalAmount { get; set; }
}