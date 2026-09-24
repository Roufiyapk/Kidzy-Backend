using Kidzy.Application.DTOs.Orders;

namespace Kidzy.Application.Interfaces.Services;

public interface IRazorpayService
{
    Task<RazorpayOrderDto>
        CreateOrderAsync(
            decimal amount,
            string receipt);

    Task<bool>
        VerifyPaymentAsync(
            string razorpayOrderId,
            string razorpayPaymentId,
            string razorpaySignature,
            decimal expectedAmount,
            string expectedReceiptPrefix);
}