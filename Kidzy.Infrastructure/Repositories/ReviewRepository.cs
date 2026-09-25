using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Domain.Entities;
using Kidzy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kidzy.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _context;

    public ReviewRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    // GET REVIEWS FOR ONE PRODUCT

    public async Task<List<Review>>
        GetByProductIdAsync(
            int productId)
    {
        return await _context.Reviews
            .Include(x => x.User)
            .Include(x => x.Product)
            .Where(x => x.ProductId == productId)
            .OrderByDescending(x => x.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    // GET ALL REVIEWS

    public async Task<List<Review>>
        GetAllAsync()
    {
        return await _context.Reviews
            .Include(x => x.User)
            .Include(x => x.Product)
            .OrderByDescending(x => x.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    // GET REVIEW BY ID

    public async Task<Review?>
        GetByIdAsync(
            int reviewId)
    {
        return await _context.Reviews
            .Include(x => x.User)
            .Include(x => x.Product)
            .FirstOrDefaultAsync(
                x => x.Id == reviewId);
    }

    // DUPLICATE REVIEW CHECK

    public async Task<bool>
        HasUserReviewedProductAsync(
            int userId,
            int productId)
    {
        return await _context.Reviews
            .AnyAsync(
                x =>
                    x.UserId == userId &&
                    x.ProductId == productId);
    }

    // PRODUCT EXISTS

    public async Task<bool>
        ProductExistsAsync(
            int productId)
    {
        return await _context.Products
            .AnyAsync(
                x => x.Id == productId);
    }

    // ADD REVIEW

    public async Task AddAsync(
        Review review)
    {
        await _context.Reviews.AddAsync(review);
    }

    // DELETE REVIEW

    public void Delete(
        Review review)
    {
        _context.Reviews.Remove(review);
    }

    // SAVE

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}