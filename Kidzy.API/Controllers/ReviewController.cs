using System.Net;
using System.Security.Claims;

using FluentValidation;

using Kidzy.API.Contracts;
using Kidzy.Application.DTOs.Reviews;
using Kidzy.Application.Interfaces.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kidzy.API.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    private readonly IValidator<CreateReviewDto>
        _createReviewValidator;

    public ReviewController(
        IReviewService reviewService,
        IValidator<CreateReviewDto> createReviewValidator)
    {
        _reviewService = reviewService;

        _createReviewValidator =
            createReviewValidator;
    }


    // ADMIN - GET ALL REVIEWS

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var reviews =
                await _reviewService.GetAllAsync();

            return Ok(
                ApiResponse<List<ReviewResponseDto>>
                    .Success(
                        reviews,
                        "All reviews retrieved successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<List<ReviewResponseDto>>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Failed to retrieve all reviews."));
        }
    }


    // USER - GET REVIEWS FOR PRODUCT

    [HttpGet("product/{productId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByProductId(
        int productId)
    {
        try
        {
            var reviews =
                await _reviewService
                    .GetByProductIdAsync(
                        productId);

            return Ok(
                ApiResponse<List<ReviewResponseDto>>
                    .Success(
                        reviews,
                        "Reviews retrieved successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<List<ReviewResponseDto>>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Failed to retrieve reviews."));
        }
    }


    // USER - GET HIGH RATED REVIEWS
    // RATING 4.5 OR ABOVE

    [HttpGet("high-rated")]
    [AllowAnonymous]
    public async Task<IActionResult> GetHighRated()
    {
        try
        {
            var reviews =
                await _reviewService
                    .GetHighRatedAsync();

            return Ok(
                ApiResponse<List<ReviewResponseDto>>
                    .Success(
                        reviews,
                        "High rated reviews retrieved successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<List<ReviewResponseDto>>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Failed to retrieve high rated reviews."));
        }
    }


    // USER - CREATE REVIEW

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(
        [FromBody] CreateReviewDto dto)
    {
        var validationResult =
            await _createReviewValidator
                .ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            return BadRequest(
                ApiResponse<ReviewResponseDto>
                    .Fail(
                        validationResult.Errors
                            .Select(
                                x => x.ErrorMessage)
                            .ToList(),
                        "Validation failed."));
        }

        try
        {
            var userId =
                GetUserId();

            var review =
                await _reviewService
                    .CreateAsync(
                        userId,
                        dto);

            return StatusCode(
                (int)HttpStatusCode.Created,

                ApiResponse<ReviewResponseDto>
                    .Success(
                        review,
                        "Review added successfully.",
                        HttpStatusCode.Created));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(
                ApiResponse<ReviewResponseDto>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Unauthorized.",
                        HttpStatusCode.Unauthorized));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<ReviewResponseDto>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Failed to add review."));
        }
    }


    // ADMIN - DELETE REVIEW

    [HttpDelete("{reviewId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(
        int reviewId)
    {
        try
        {
            await _reviewService
                .DeleteAsync(reviewId);

            return Ok(
                ApiResponse<string>
                    .Success(
                        string.Empty,
                        "Review deleted successfully."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(
                ApiResponse<string>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Review not found.",
                        HttpStatusCode.NotFound));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<string>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Failed to delete review."));
        }
    }


    // GET USER ID FROM JWT

    private int GetUserId()
    {
        var userId =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedAccessException(
                "User ID not found in token.");
        }

        if (!int.TryParse(
                userId,
                out var parsedUserId))
        {
            throw new UnauthorizedAccessException(
                "Invalid user ID in token.");
        }

        return parsedUserId;
    }
}