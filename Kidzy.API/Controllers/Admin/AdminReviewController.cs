using System.Net;
using Kidzy.API.Contracts;
using Kidzy.Application.DTOs.Reviews;
using Kidzy.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kidzy.API.Controllers.Admin;

[ApiController]
[Route("api/admin/reviews")]
[Authorize(Roles = "Admin")]
public class AdminReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public AdminReviewController(
        IReviewService reviewService)
    {
        _reviewService =
            reviewService;
    }

    // GET ALL REVIEWS

    [HttpGet]
    public async Task<IActionResult>
        GetAll()
    {
        try
        {
            var reviews =
                await _reviewService
                    .GetAllAsync();

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

    // DELETE REVIEW

    [HttpDelete("{id:int}")]
    public async Task<IActionResult>
        Delete(
            int id)
    {
        try
        {
            var deleted =
                await _reviewService
                    .DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(
                    ApiResponse<string>
                        .Fail(
                            new List<string>
                            {
                                "Review not found."
                            },
                            "Review not found.",
                            HttpStatusCode.NotFound));
            }

            return Ok(
                ApiResponse<string>
                    .Success(
                        string.Empty,
                        "Review deleted successfully."));
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
}