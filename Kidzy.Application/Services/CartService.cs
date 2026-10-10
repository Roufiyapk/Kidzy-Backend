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

    public async Task<CartResponseDto> GetCartAsync(int userId)
    {
        var cart = await _cartRepository
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
            TotalAmount = items.Sum(x => x.TotalPrice)
        };
    }

    // ADD TO CART

    public async Task<CartItemDto> AddToCartAsync(
        int userId,
        AddToCartDto dto)
    {
        // Validate quantity

        if (dto.Quantity <= 0)
        {
            throw new Exception(
                "Quantity must be greater than zero.");
        }

        // Get product with variants

        var product = await _cartRepository
            .GetProductWithVariantsAsync(dto.ProductId);

        if (product == null)
        {
            throw new Exception("Product not found.");
        }

        // Check whether product has variants

        var hasVariants =
            product.Variants != null &&
            product.Variants.Any();

        if (hasVariants && !dto.ProductVariantId.HasValue)
        {
            throw new Exception(
                "Please select a size.");
        }

        ProductVariant? variant = null;

        // Validate selected variant

        if (dto.ProductVariantId.HasValue)
        {
            variant = product.Variants
                .FirstOrDefault(v =>
                    v.Id == dto.ProductVariantId.Value);

            if (variant == null)
            {
                throw new Exception(
                    "Invalid product variant.");
            }
        }

        // Determine available stock

        var availableStock = variant != null
            ? variant.Stock
            : product.Stock;

        if (availableStock <= 0)
        {
            throw new Exception(
                "This product is currently out of stock.");
        }

        // Get user's cart

        var cart = await _cartRepository
            .GetCartAsync(userId);

        // Create cart if it does not exist

        if (cart == null)
        {
            cart = await _cartRepository
                .CreateCartAsync(userId);
        }


        var existingItem = await _cartRepository
            .GetExistingCartItemAsync(
                userId,
                dto.ProductId,
                dto.ProductVariantId);

        if (existingItem != null)
        {

            var newQuantity =
                existingItem.Quantity + dto.Quantity;


            if (newQuantity > availableStock)
            {
                throw new Exception(
                    $"Only {availableStock} items are available for {product.Name}.");
            }

            existingItem.Quantity = newQuantity;

            _cartRepository.UpdateCartItem(existingItem);

            await _cartRepository.SaveChangesAsync();

            return MapToDto(existingItem);
        }

        // Validate quantity for a new cart item

        if (dto.Quantity > availableStock)
        {
            throw new Exception(
                $"Only {availableStock} items are available for {product.Name}.");
        }

        // Create new cart item

        var cartItem = new CartItem
        {
            CartId = cart.Id,
            ProductId = dto.ProductId,
            ProductVariantId = dto.ProductVariantId,
            Quantity = dto.Quantity
        };

        await _cartRepository.AddCartItemAsync(cartItem);

        await _cartRepository.SaveChangesAsync();

        // Reload complete cart item

        var createdItem = await _cartRepository
            .GetCartItemAsync(userId, cartItem.Id);

        if (createdItem == null)
        {
            throw new Exception(
                "Failed to create cart item.");
        }

        return MapToDto(createdItem);
    }

    // UPDATE QUANTITY

    public async Task<CartItemDto?> UpdateQuantityAsync(
        int userId,
        int cartItemId,
        UpdateCartItemDto dto)
    {
        // Validate quantity

        if (dto.Quantity <= 0)
        {
            throw new Exception(
                "Quantity must be greater than zero.");
        }

        // Get cart item with product and variant

        var item = await _cartRepository
            .GetCartItemAsync(userId, cartItemId);

        if (item == null)
        {
            return null;
        }


        var availableStock = item.ProductVariant != null
            ? item.ProductVariant.Stock
            : item.Product.Stock;


        if (dto.Quantity > availableStock)
        {
            throw new Exception(
                $"Only {availableStock} items are available for {item.Product.Name}.");
        }

        // Update quantity

        item.Quantity = dto.Quantity;

        _cartRepository.UpdateCartItem(item);

        await _cartRepository.SaveChangesAsync();

        return MapToDto(item);
    }

    // REMOVE ITEM

    public async Task<bool> RemoveFromCartAsync(
        int userId,
        int cartItemId)
    {
        var item = await _cartRepository
            .GetCartItemAsync(userId, cartItemId);

        if (item == null)
        {
            return false;
        }

        _cartRepository.RemoveCartItem(item);

        await _cartRepository.SaveChangesAsync();

        return true;
    }

    // CLEAR CART

    public async Task ClearCartAsync(int userId)
    {
        var cart = await _cartRepository
            .GetCartAsync(userId);

        if (cart == null)
        {
            return;
        }

        _cartRepository.RemoveCartItems(cart.Items);

        await _cartRepository.SaveChangesAsync();
    }

    // MAP ENTITY TO DTO
    private static CartItemDto MapToDto(CartItem item)
    {
        var imageUrl = item.Product.ImageUrl ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(imageUrl))
        {
            try
            {
                var parsedImages =
                    System.Text.Json.JsonSerializer.Deserialize<string[]>(imageUrl);

                if (parsedImages != null && parsedImages.Length > 0)
                {
                    imageUrl = parsedImages[0] ?? string.Empty;
                }
            }
            catch (System.Text.Json.JsonException)
            {
            }
        }

        return new CartItemDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductVariantId = item.ProductVariantId,
            ProductName = item.Product.Name,
            Price = item.Product.Price,
            ImageUrl = imageUrl,
            AgeGroup = item.ProductVariant?.AgeGroup,
            Size = item.ProductVariant?.Size,
            Quantity = item.Quantity,
            TotalPrice = item.Product.Price * item.Quantity
        };
    }

}