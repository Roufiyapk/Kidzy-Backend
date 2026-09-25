using Kidzy.Application.DTOs.Reviews;

namespace Kidzy.Application.Interfaces.Services;

public interface IReviewService
{
    Task<ReviewResponseDto> CreateAsync(
        int userId,
        CreateReviewDto dto);

    Task<List<ReviewResponseDto>>
        GetByProductIdAsync(
            int productId);

    Task<List<ReviewResponseDto>>
        GetAllAsync();

    Task<bool> DeleteAsync(
        int reviewId);
}