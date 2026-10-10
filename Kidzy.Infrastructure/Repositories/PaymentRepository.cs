using Kidzy.Application.Interfaces.Repositories;

using Kidzy.Domain.Entities;

using Kidzy.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

using PaymentEntity = Kidzy.Domain.Entities.Payment;

namespace Kidzy.Infrastructure.Repositories;

public class PaymentRepository
    : IPaymentRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentEntity?> GetByIdAsync(
        int paymentId)
    {
        return await _context.Payments
            .Include(x => x.User)
            .FirstOrDefaultAsync(
                x => x.Id == paymentId);
    }

    public async Task<PaymentEntity?> GetByRazorpayOrderIdAsync(
        string razorpayOrderId)
    {
        return await _context.Payments
            .Include(x => x.User)
            .FirstOrDefaultAsync(
                x =>
                    x.RazorpayOrderId ==
                    razorpayOrderId);
    }

    public async Task<bool> IsPaymentAlreadyUsedAsync(
        string razorpayPaymentId)
    {
        return await _context.Payments
            .AnyAsync(
                x =>
                    x.RazorpayPaymentId ==
                    razorpayPaymentId);
    }

    public async Task AddAsync(
        PaymentEntity payment)
    {
        await _context.Payments.AddAsync(payment);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}