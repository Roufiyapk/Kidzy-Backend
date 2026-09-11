namespace Kidzy.Application.DTOs.Wishlist
{
    public class WishlistResponseDto
    {
        public int WishlistId { get; set; }

        public int UserId { get; set; }

        public List<WishlistItemResponseDto> Items { get; set; }
            = new List<WishlistItemResponseDto>();
    }
}