namespace Kidzy.Domain.Entities
{
    public class Cart
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        // User relationship
        public User User { get; set; } = null!;

        // Cart → CartItems
        public ICollection<CartItem> CartItems { get; set; }
            = new List<CartItem>();
    }
}