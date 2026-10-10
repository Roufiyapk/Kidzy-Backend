using Kidzy.Domain.Entities;

namespace Kidzy.Application.Interfaces.Repositories;

public interface IPaymentRepository
{
    Task<Payment?>
        GetByIdAsync(
            int paymentId);

    Task<Payment?>
        GetByRazorpayOrderIdAsync(
            string razorpayOrderId);

    Task<bool>
        IsPaymentAlreadyUsedAsync(
            string razorpayPaymentId);

    Task AddAsync(
        Payment payment);

    Task SaveChangesAsync();
}