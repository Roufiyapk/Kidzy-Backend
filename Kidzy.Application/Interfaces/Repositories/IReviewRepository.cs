using Kidzy.Domain.Entities;

namespace Kidzy.Application.Interfaces.Repositories;

public interface IReviewRepository
{
    Task<List<Review>> GetByProductIdAsync(
        int productId);

    Task<List<Review>> GetAllAsync();

    Task<Review?> GetByIdAsync(
        int reviewId);

    Task<bool> HasUserReviewedProductAsync(
        int userId,
        int productId);

    Task<bool> ProductExistsAsync(
        int productId);

    Task AddAsync(
        Review review);

    void Delete(
        Review review);

    Task SaveChangesAsync();
}