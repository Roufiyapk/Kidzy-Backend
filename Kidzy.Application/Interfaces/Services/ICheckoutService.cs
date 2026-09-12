using Kidzy.Application.DTOs.Checkout;
using Kidzy.Application.DTOs.Order;

namespace Kidzy.Application.Interfaces.Services
{
    public interface ICheckoutService
    {
        Task<OrderResponseDto?> CheckoutAsync(
            int userId,
            CheckoutDto dto);
    }
}