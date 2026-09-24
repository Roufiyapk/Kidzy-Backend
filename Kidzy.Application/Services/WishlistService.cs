using Kidzy.Application.DTOs.Wishlist;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;
using Kidzy.Domain.Entities;

namespace Kidzy.Application.Services;

public class WishlistService : IWishlistService
{
    private readonly IWishlistRepository _wishlistRepository;

    public WishlistService(
        IWishlistRepository wishlistRepository)
    {
        _wishlistRepository =
            wishlistRepository;
    }

    public async Task<WishlistResponseDto>
        GetWishlistAsync(int userId)
    {
        var wishlist =
            await _wishlistRepository
                .GetWishlistAsync(userId);

        if (wishlist == null)
        {
            return new WishlistResponseDto();
        }

        return new WishlistResponseDto
        {
            WishlistId = wishlist.Id,
            Items = wishlist.Items
                .Select(MapToDto)
                .ToList()
        };
    }

    public async Task<WishlistItemDto>
        AddToWishlistAsync(
            int userId,
            AddToWishlistDto dto)
    {
        var product =
            await _wishlistRepository
                .GetProductAsync(
                    dto.ProductId);

        if (product == null)
        {
            throw new Exception(
                "Product not found.");
        }

        var existingItem =
            await _wishlistRepository
                .GetWishlistItemAsync(
                    userId,
                    dto.ProductId);

        if (existingItem != null)
        {
            return MapToDto(existingItem);
        }

        var wishlist =
            await _wishlistRepository
                .GetWishlistAsync(userId);

        if (wishlist == null)
        {
            wishlist =
                await _wishlistRepository
                    .CreateWishlistAsync(userId);
        }

        var wishlistItem =
            new WishlistItem
            {
                WishlistId = wishlist.Id,
                ProductId = product.Id
            };

        await _wishlistRepository
            .AddItemAsync(wishlistItem);

        await _wishlistRepository
            .SaveChangesAsync();

        wishlistItem.Product = product;

        return MapToDto(wishlistItem);
    }

    public async Task<bool>
        RemoveFromWishlistAsync(
            int userId,
            int productId)
    {
        var item =
            await _wishlistRepository
                .GetWishlistItemAsync(
                    userId,
                    productId);

        if (item == null)
        {
            return false;
        }

        _wishlistRepository
            .RemoveItem(item);

        await _wishlistRepository
            .SaveChangesAsync();

        return true;
    }

    private static WishlistItemDto MapToDto(
        WishlistItem item)
    {
        return new WishlistItemDto
        {
            Id = item.Id,

            ProductId =
                item.ProductId,

            ProductName =
                item.Product.Name,

            Price =
                item.Product.Price,

            ImageUrl =
                item.Product.ImageUrl,

            BestSeller =
                item.Product.BestSeller,

            NewArrival =
                item.Product.NewArrival
        };
    }
}