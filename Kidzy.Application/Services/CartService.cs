using Kidzy.Application.DTOs.Cart;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;
using Kidzy.Domain.Entities;

namespace Kidzy.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(
            ICartRepository cartRepository,
            IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        // GET CART
        public async Task<CartResponseDto?> GetCartAsync(int userId)
        {
            var cart =
                await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
            {
                return null;
            }

            return MapToDto(cart);
        }

        // ADD TO CART
        public async Task<CartResponseDto?> AddToCartAsync(
            int userId,
            AddToCartDto dto)
        {
            if (dto.Quantity <= 0)
            {
                return null;
            }

            var product =
                await _productRepository.GetByIdAsync(dto.ProductId);

            if (product == null)
            {
                return null;
            }

            var productSize = product.ProductSizes
                .FirstOrDefault(ps =>
                    ps.Size == dto.SelectedSize);

            if (productSize == null)
            {
                return null;
            }

            if (dto.Quantity > productSize.Stock)
            {
                return null;
            }

            var cart =
                await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId
                };

                cart =
                    await _cartRepository.CreateAsync(cart);
            }

            var existingItem =
                await _cartRepository.GetItemAsync(
                    cart.Id,
                    dto.ProductId,
                    dto.SelectedSize);

            if (existingItem != null)
            {
                var newQuantity =
                    existingItem.Quantity + dto.Quantity;

                if (newQuantity > productSize.Stock)
                {
                    return null;
                }

                existingItem.Quantity = newQuantity;

                await _cartRepository.UpdateItemAsync(
                    existingItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = dto.ProductId,
                    SelectedSize = dto.SelectedSize,
                    Quantity = dto.Quantity
                };

                await _cartRepository.AddItemAsync(cartItem);
            }

            var updatedCart =
                await _cartRepository.GetByUserIdAsync(userId);

            if (updatedCart == null)
            {
                return null;
            }

            return MapToDto(updatedCart);
        }

        // UPDATE CART ITEM
        public async Task<CartResponseDto?> UpdateCartItemAsync(
            int userId,
            int itemId,
            UpdateCartItemDto dto)
        {
            if (dto.Quantity <= 0)
            {
                return null;
            }

            var cart =
                await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
            {
                return null;
            }

            var item = cart.CartItems
                .FirstOrDefault(ci => ci.Id == itemId);

            if (item == null)
            {
                return null;
            }

            var product =
                await _productRepository.GetByIdAsync(
                    item.ProductId);

            if (product == null)
            {
                return null;
            }

            var productSize = product.ProductSizes
                .FirstOrDefault(ps =>
                    ps.Size == item.SelectedSize);

            if (productSize == null)
            {
                return null;
            }

            if (dto.Quantity > productSize.Stock)
            {
                return null;
            }

            item.Quantity = dto.Quantity;

            await _cartRepository.UpdateItemAsync(item);

            var updatedCart =
                await _cartRepository.GetByUserIdAsync(userId);

            if (updatedCart == null)
            {
                return null;
            }

            return MapToDto(updatedCart);
        }

        // DELETE CART ITEM
        public async Task<bool> DeleteCartItemAsync(
            int userId,
            int itemId)
        {
            var cart =
                await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
            {
                return false;
            }

            var item = cart.CartItems
                .FirstOrDefault(ci => ci.Id == itemId);

            if (item == null)
            {
                return false;
            }

            await _cartRepository.DeleteItemAsync(item);

            return true;
        }

        // CLEAR CART
        public async Task<bool> ClearCartAsync(int userId)
        {
            var cart =
                await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
            {
                return false;
            }

            await _cartRepository.ClearAsync(cart);

            return true;
        }

        // MAPPING
        private static CartResponseDto MapToDto(
            Cart cart)
        {
            var items = cart.CartItems
                .Select(item => new CartItemResponseDto
                {
                    Id = item.Id,

                    ProductId = item.ProductId,

                    ProductName = item.Product.Name,

                    Price = item.Product.Price,

                    Image = item.Product.Image,

                    SelectedSize = item.SelectedSize,

                    Quantity = item.Quantity,

                    SubTotal =
                        item.Product.Price * item.Quantity

                })
                .ToList();

            return new CartResponseDto
            {
                CartId = cart.Id,

                UserId = cart.UserId,

                Items = items,

                TotalPrice =
                    items.Sum(item => item.SubTotal)
            };
        }
    }
}