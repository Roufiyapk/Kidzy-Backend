using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Domain.Entities;
using Kidzy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kidzy.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly ApplicationDbContext _context;

    public CartRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }


    // GET CART

    public async Task<Cart?> GetCartAsync(
        int userId)
    {
        return await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)

            .Include(c => c.Items)
                .ThenInclude(i => i.ProductVariant)

            .FirstOrDefaultAsync(
                c => c.UserId == userId);
    }


    // CREATE CART

    public async Task<Cart> CreateCartAsync(
        int userId)
    {
        var cart = new Cart
        {
            UserId = userId
        };

        await _context.Carts.AddAsync(cart);

        await SaveChangesAsync();

        return cart;
    }


    // GET PRODUCT WITH VARIANTS

    public async Task<Product?>
        GetProductWithVariantsAsync(
            int productId)
    {
        return await _context.Products
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(
                p => p.Id == productId);
    }


    // GET CART ITEM

    public async Task<CartItem?>
        GetCartItemAsync(
            int userId,
            int cartItemId)
    {
        return await _context.CartItems

            .Include(i => i.Cart)

            .Include(i => i.Product)

            .Include(i => i.ProductVariant)

            .FirstOrDefaultAsync(i =>
                i.Id == cartItemId &&
                i.Cart.UserId == userId);
    }


    // GET EXISTING CART ITEM

    public async Task<CartItem?>
        GetExistingCartItemAsync(
            int userId,
            int productId,
            int? productVariantId)
    {
        return await _context.CartItems

            .Include(i => i.Cart)

            .Include(i => i.Product)

            .Include(i => i.ProductVariant)

            .FirstOrDefaultAsync(i =>
                i.Cart.UserId == userId &&
                i.ProductId == productId &&
                i.ProductVariantId == productVariantId);
    }


    // ADD CART ITEM

    public async Task AddCartItemAsync(
        CartItem cartItem)
    {
        await _context.CartItems
            .AddAsync(cartItem);
    }


    // UPDATE

    public void UpdateCartItem(
        CartItem cartItem)
    {
        _context.CartItems.Update(
            cartItem);
    }


    // REMOVE ONE

    public void RemoveCartItem(
        CartItem cartItem)
    {
        _context.CartItems.Remove(
            cartItem);
    }


    // REMOVE ALL

    public void RemoveCartItems(
        IEnumerable<CartItem> cartItems)
    {
        _context.CartItems.RemoveRange(
            cartItems);
    }


    // SAVE

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}