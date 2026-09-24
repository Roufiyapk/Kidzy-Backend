using Kidzy.Application.DTOs.Cart;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;
using Kidzy.Domain.Entities;

namespace Kidzy.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;

    public CartService(
        ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }


    // GET CART

    public async Task<CartResponseDto>
        GetCartAsync(int userId)
    {
        var cart =
            await _cartRepository
                .GetCartAsync(userId);

        if (cart == null)
        {
            return new CartResponseDto();
        }

        var items = cart.Items
            .Select(MapToDto)
            .ToList();

        return new CartResponseDto
        {
            CartId = cart.Id,

            Items = items,

            TotalAmount =
                items.Sum(x => x.TotalPrice)
        };
    }


    // =====================================
    // ADD TO CART
    // =====================================

    public async Task<CartItemDto>
        AddToCartAsync(
            int userId,
            AddToCartDto dto)
    {
        // Quantity validation
        if (dto.Quantity <= 0)
        {
            throw new Exception(
                "Quantity must be greater than zero.");
        }


        // Get product + variants
        var product =
            await _cartRepository
                .GetProductWithVariantsAsync(
                    dto.ProductId);

        if (product == null)
        {
            throw new Exception(
                "Product not found.");
        }


        // Does product have sizes?
        var hasVariants =
            product.Variants != null &&
            product.Variants.Any();


        // If sizes exist,
        // user MUST select a size
        if (hasVariants &&
            dto.ProductVariantId == null)
        {
            throw new Exception(
                "Please select a size.");
        }


        ProductVariant? variant = null;


        // Validate selected variant
        if (dto.ProductVariantId.HasValue)
        {
            variant =
                product.Variants
                    .FirstOrDefault(v =>
                        v.Id ==
                        dto.ProductVariantId.Value);

            if (variant == null)
            {
                throw new Exception(
                    "Invalid product variant.");
            }


            // Check stock
            if (dto.Quantity >
                variant.Stock)
            {
                throw new Exception(
                    "Not enough stock available.");
            }
        }


        // Get user's cart
        var cart =
            await _cartRepository
                .GetCartAsync(userId);


        // Create cart if needed
        if (cart == null)
        {
            cart =
                await _cartRepository
                    .CreateCartAsync(userId);
        }


        // Check same product + same variant
        var existingItem =
            await _cartRepository
                .GetExistingCartItemAsync(
                    userId,
                    dto.ProductId,
                    dto.ProductVariantId);


        if (existingItem != null)
        {
            var newQuantity =
                existingItem.Quantity +
                dto.Quantity;


            // Check stock
            if (variant != null &&
                newQuantity > variant.Stock)
            {
                throw new Exception(
                    "Requested quantity exceeds available stock.");
            }


            existingItem.Quantity =
                newQuantity;

            _cartRepository.UpdateCartItem(
                existingItem);

            await _cartRepository
                .SaveChangesAsync();

            return MapToDto(existingItem);
        }


        // Create new cart item
        var cartItem = new CartItem
        {
            CartId = cart.Id,

            ProductId =
                dto.ProductId,

            ProductVariantId =
                dto.ProductVariantId,

            Quantity =
                dto.Quantity
        };


        await _cartRepository
            .AddCartItemAsync(cartItem);

        await _cartRepository
            .SaveChangesAsync();


        // Get complete item with Product
        // and ProductVariant
        var createdItem =
            await _cartRepository
                .GetCartItemAsync(
                    userId,
                    cartItem.Id);

        if (createdItem == null)
        {
            throw new Exception(
                "Failed to create cart item.");
        }


        return MapToDto(createdItem);
    }


    // UPDATE QUANTITY

    public async Task<CartItemDto?>
        UpdateQuantityAsync(
            int userId,
            int cartItemId,
            UpdateCartItemDto dto)
    {
        if (dto.Quantity <= 0)
        {
            throw new Exception(
                "Quantity must be greater than zero.");
        }


        var item =
            await _cartRepository
                .GetCartItemAsync(
                    userId,
                    cartItemId);


        if (item == null)
        {
            return null;
        }


        // Check variant stock
        if (item.ProductVariant != null)
        {
            if (dto.Quantity >
                item.ProductVariant.Stock)
            {
                throw new Exception(
                    "Requested quantity exceeds available stock.");
            }
        }


        item.Quantity =
            dto.Quantity;


        _cartRepository.UpdateCartItem(
            item);

        await _cartRepository
            .SaveChangesAsync();


        return MapToDto(item);
    }


    // REMOVE ITEM

    public async Task<bool>
        RemoveFromCartAsync(
            int userId,
            int cartItemId)
    {
        var item =
            await _cartRepository
                .GetCartItemAsync(
                    userId,
                    cartItemId);


        if (item == null)
        {
            return false;
        }


        _cartRepository.RemoveCartItem(
            item);

        await _cartRepository
            .SaveChangesAsync();


        return true;
    }


    // CLEAR CART

    public async Task ClearCartAsync(
        int userId)
    {
        var cart =
            await _cartRepository
                .GetCartAsync(userId);


        if (cart == null)
        {
            return;
        }


        _cartRepository.RemoveCartItems(
            cart.Items);

        await _cartRepository
            .SaveChangesAsync();
    }


    // =====================================
    // MAP ENTITY → DTO
    // =====================================

    private static CartItemDto MapToDto(
        CartItem item)
    {
        return new CartItemDto
        {
            Id = item.Id,

            ProductId =
                item.ProductId,

            ProductVariantId =
                item.ProductVariantId,

            ProductName =
                item.Product.Name,

            Price =
                item.Product.Price,

            ImageUrl =
                item.Product.ImageUrl
                ?? string.Empty,

            AgeGroup =
                item.ProductVariant?.AgeGroup,

            Size =
                item.ProductVariant?.Size,

            Quantity =
                item.Quantity,

            TotalPrice =
                item.Product.Price *
                item.Quantity
        };
    }
}