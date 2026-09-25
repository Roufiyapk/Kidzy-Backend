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

    public async Task<ReviewResponseDto>
        CreateAsync(
            int userId,
            CreateReviewDto dto)
    {
        // Check product
        var productExists =
            await _reviewRepository
                .ProductExistsAsync(
                    dto.ProductId);

        if (!productExists)
        {
            throw new Exception(
                "Product not found.");
        }

        // Check duplicate review
        var alreadyReviewed =
            await _reviewRepository
                .HasUserReviewedProductAsync(
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

            Title =
                string.IsNullOrWhiteSpace(dto.Title)
                    ? null
                    : dto.Title.Trim(),

            Comment =
                dto.Comment.Trim(),

            CreatedAt =
                DateTime.UtcNow
        };

        await _reviewRepository
            .AddAsync(review);

        await _reviewRepository
            .SaveChangesAsync();

        // Get created review
        var createdReview =
            await _reviewRepository
                .GetByIdAsync(review.Id);

        if (createdReview == null)
        {
            throw new Exception(
                "Failed to retrieve created review.");
        }

        return MapToDto(createdReview);
    }

    // GET PRODUCT REVIEWS

    public async Task<List<ReviewResponseDto>>
        GetByProductIdAsync(
            int productId)
    {
        var reviews =
            await _reviewRepository
                .GetByProductIdAsync(
                    productId);

        return reviews
            .Select(MapToDto)
            .ToList();
    }

    // GET ALL REVIEWS

    public async Task<List<ReviewResponseDto>>
        GetAllAsync()
    {
        var reviews =
            await _reviewRepository
                .GetAllAsync();

        return reviews
            .Select(MapToDto)
            .ToList();
    }

    
    // DELETE REVIEW

    public async Task<bool>
        DeleteAsync(
            int reviewId)
    {
        var review =
            await _reviewRepository
                .GetByIdAsync(
                    reviewId);

        if (review == null)
        {
            return false;
        }

        _reviewRepository.Delete(review);

        await _reviewRepository
            .SaveChangesAsync();

        return true;
    }

    // ENTITY -> DTO

    private static ReviewResponseDto
        MapToDto(
            Review review)
    {
        return new ReviewResponseDto
        {
            Id = review.Id,

            ProductId =
                review.ProductId,

            UserId =
                review.UserId,

            UserName =
                review.User?.Name ??
                "Customer",

            Rating =
                review.Rating,

            Title =
                review.Title,

            Comment =
                review.Comment,

            ProductName =
                review.Product?.Name ??
                string.Empty,

            ProductImage =
                review.Product?.ImageUrl ??
                string.Empty,

            CreatedAt =
                review.CreatedAt
        };
    }
}