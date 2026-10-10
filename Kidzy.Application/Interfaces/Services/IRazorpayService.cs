using Kidzy.Application.DTOs.Payments;

namespace Kidzy.Application.Interfaces.Services;

public interface IRazorpayService
{
    Task<RazorpayOrderResponseDto>
        CreateOrderAsync(
            decimal amount,
            string receipt);

    Task<bool>
        VerifyPaymentAsync(
            string razorpayOrderId,
            string razorpayPaymentId,
            string razorpaySignature);
}