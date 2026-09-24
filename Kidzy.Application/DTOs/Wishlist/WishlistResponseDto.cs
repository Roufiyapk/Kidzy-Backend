namespace Kidzy.Application.DTOs.Wishlist;

public class WishlistResponseDto
{
    public int WishlistId { get; set; }

    public List<WishlistItemDto> Items { get; set; } = new();
}