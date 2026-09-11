using Kidzy.Application.DTOs.Wishlist;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;
using Kidzy.Domain.Entities;

namespace Kidzy.Application.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IProductRepository _productRepository;

        public WishlistService(
            IWishlistRepository wishlistRepository,
            IProductRepository productRepository)
        {
            _wishlistRepository = wishlistRepository;
            _productRepository = productRepository;
        }

        public async Task<WishlistResponseDto?> GetWishlistAsync(
            int userId)
        {
            var wishlist =
                await _wishlistRepository.GetByUserIdAsync(userId);

            if (wishlist == null)
            {
                return null;
            }

            return MapToDto(wishlist);
        }

        public async Task<WishlistResponseDto?> AddToWishlistAsync(
            int userId,
            AddToWishlistDto dto)
        {
            // Check whether product exists
            var product =
                await _productRepository.GetByIdAsync(dto.ProductId);

            if (product == null)
            {
                return null;
            }

            // Find user's wishlist
            var wishlist =
                await _wishlistRepository.GetByUserIdAsync(userId);

            // Create wishlist if it doesn't exist
            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    UserId = userId
                };

                wishlist =
                    await _wishlistRepository.CreateAsync(wishlist);
            }

            // Check whether product already exists
            var existingItem =
                await _wishlistRepository.GetItemAsync(
                    wishlist.Id,
                    dto.ProductId);

            if (existingItem == null)
            {
                var wishlistItem = new WishlistItem
                {
                    WishlistId = wishlist.Id,
                    ProductId = dto.ProductId
                };

                await _wishlistRepository.AddItemAsync(
                    wishlistItem);
            }

            // Get updated wishlist
            var updatedWishlist =
                await _wishlistRepository.GetByUserIdAsync(userId);

            if (updatedWishlist == null)
            {
                return null;
            }

            return MapToDto(updatedWishlist);
        }

        public async Task<bool> RemoveFromWishlistAsync(
            int userId,
            int productId)
        {
            var wishlist =
                await _wishlistRepository.GetByUserIdAsync(userId);

            if (wishlist == null)
            {
                return false;
            }

            var item = wishlist.WishlistItems
                .FirstOrDefault(x => x.ProductId == productId);

            if (item == null)
            {
                return false;
            }

            await _wishlistRepository.DeleteItemAsync(item);

            return true;
        }

        public async Task<bool> ClearWishlistAsync(
            int userId)
        {
            var wishlist =
                await _wishlistRepository.GetByUserIdAsync(userId);

            if (wishlist == null)
            {
                return false;
            }

            await _wishlistRepository.ClearAsync(wishlist);

            return true;
        }

        private static WishlistResponseDto MapToDto(
            Wishlist wishlist)
        {
            var items = wishlist.WishlistItems
                .Select(item => new WishlistItemResponseDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Price = item.Product.Price,
                    Image = item.Product.Image
                })
                .ToList();

            return new WishlistResponseDto
            {
                WishlistId = wishlist.Id,
                UserId = wishlist.UserId,
                Items = items
            };
        }
    }
}