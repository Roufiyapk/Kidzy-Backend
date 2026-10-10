using System.Text.Json;

using Kidzy.Application.DTOs.Reviews;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;
using Kidzy.Domain.Entities;

namespace Kidzy.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;

    public ReviewService(
        IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    // CREATE REVIEW

    public async Task<ReviewResponseDto> CreateAsync(
        int userId,
        CreateReviewDto dto)
    {
        // Check whether the product exists

        var productExists =
            await _reviewRepository.ProductExistsAsync(
                dto.ProductId);

        if (!productExists)
        {
            throw new Exception("Product not found.");
        }

        // Check whether the user has already reviewed this product
        var alreadyReviewed =
            await _reviewRepository.HasUserReviewedProductAsync(
                userId,
                dto.ProductId);

        if (alreadyReviewed)
        {
            throw new Exception(
                "You have already reviewed this product.");
        }

        // Create review
        var review = new Review
        {
            ProductId = dto.ProductId,
            UserId = userId,
            Rating = dto.Rating,

            Title = string.IsNullOrWhiteSpace(dto.Title)
                ? null
                : dto.Title.Trim(),

            Comment = dto.Comment.Trim(),

            CreatedAt = DateTime.UtcNow
        };

        // Save review
        await _reviewRepository.AddAsync(review);

        await _reviewRepository.SaveChangesAsync();

        // Retrieve the created review
        var createdReview =
            await _reviewRepository.GetByIdAsync(
                review.Id);

        if (createdReview == null)
        {
            throw new Exception(
                "Failed to retrieve created review.");
        }

        return MapToDto(createdReview);
    }

    // GET REVIEWS BY PRODUCT ID

    public async Task<List<ReviewResponseDto>> GetByProductIdAsync(
        int productId)
    {
        var reviews =
            await _reviewRepository.GetByProductIdAsync(
                productId);

        return reviews
            .Select(MapToDto)
            .ToList();
    }

    // GET ALL REVIEWS

    public async Task<List<ReviewResponseDto>> GetAllAsync()
    {
        var reviews =
            await _reviewRepository.GetAllAsync();

        return reviews
            .Select(MapToDto)
            .ToList();
    }

    // GET HIGH-RATED REVIEWS
    // RATING 4.5 OR ABOVE

    public async Task<List<ReviewResponseDto>> GetHighRatedAsync()
    {
        var reviews =
            await _reviewRepository.GetHighRatedAsync();

        return reviews
            .Select(MapToDto)
            .ToList();
    }

    // DELETE REVIEW

    public async Task<bool> DeleteAsync(
        int reviewId)
    {
        var review =
            await _reviewRepository.GetByIdAsync(
                reviewId);

        if (review == null)
        {
            return false;
        }

        _reviewRepository.Delete(review);

        await _reviewRepository.SaveChangesAsync();

        return true;
    }

    

    private static ReviewResponseDto MapToDto(
        Review review)
    {
        var imageUrl =
            review.Product?.ImageUrl ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(imageUrl))
        {
            try
            {
                var parsedImages =
                    JsonSerializer.Deserialize<string[]>(
                        imageUrl);

                if (parsedImages != null &&
                    parsedImages.Length > 0)
                {
                    imageUrl =
                        parsedImages.FirstOrDefault(
                            image => !string.IsNullOrWhiteSpace(image))
                        ?? string.Empty;
                }
            }
            catch (JsonException)
            {
            }
        }

        return new ReviewResponseDto
        {
            Id = review.Id,

            ProductId = review.ProductId,

            UserId = review.UserId,

            UserName =
                review.User?.Name ?? "Customer",

            Rating = review.Rating,

            Title = review.Title,

            Comment = review.Comment,

            ProductName =
                review.Product?.Name ?? string.Empty,

            ProductImage = imageUrl,

            CreatedAt = review.CreatedAt
        };
    }
}