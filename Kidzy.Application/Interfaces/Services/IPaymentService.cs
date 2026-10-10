using Kidzy.Application.DTOs.Payments;

namespace Kidzy.Application.Interfaces.Services;

public interface IPaymentService
{
    Task<PaymentResponseDto>
        CreateOrderAsync(
            int userId,
            CreatePaymentOrderDto dto);

    Task<PaymentResponseDto>
        VerifyPaymentAsync(
            int userId,
            VerifyPaymentDto dto);

    Task<PaymentResponseDto?>
        GetPaymentAsync(
            int userId,
            int paymentId);
}