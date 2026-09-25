using System.Net;
using System.Security.Claims;
using FluentValidation;
using Kidzy.API.Contracts;
using Kidzy.Application.DTOs.Profile;
using Kidzy.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kidzy.API.Controllers.User;

[ApiController]
[Route("api/users")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IUserService _userService;

    private readonly IValidator<UpdateProfileDto>
        _updateProfileValidator;

    public ProfileController(
        IUserService userService,
        IValidator<UpdateProfileDto>
            updateProfileValidator)
    {
        _userService = userService;

        _updateProfileValidator =
            updateProfileValidator;
    }

    // GET PROFILE

    [HttpGet("profile")]
    public async Task<IActionResult>
        GetProfile()
    {
        try
        {
            var userId =
                GetUserId();

            var profile =
                await _userService
                    .GetProfileAsync(userId);

            if (profile == null)
            {
                return NotFound(
                    ApiResponse<ProfileResponseDto>
                        .Fail(
                            new List<string>
                            {
                                "User not found."
                            },
                            "User not found.",
                            HttpStatusCode.NotFound));
            }

            return Ok(
                ApiResponse<ProfileResponseDto>
                    .Success(
                        profile,
                        "Profile retrieved successfully."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(
                ApiResponse<ProfileResponseDto>
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
                ApiResponse<ProfileResponseDto>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Failed to retrieve profile."));
        }
    }

    // UPDATE PROFILE

    [HttpPut("profile")]
    public async Task<IActionResult>
        UpdateProfile(
            [FromBody] UpdateProfileDto dto)
    {
        // FluentValidation
        var validationResult =
            await _updateProfileValidator
                .ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            return BadRequest(
                ApiResponse<ProfileResponseDto>
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

            var updatedProfile =
                await _userService
                    .UpdateProfileAsync(
                        userId,
                        dto);

            return Ok(
                ApiResponse<ProfileResponseDto>
                    .Success(
                        updatedProfile,
                        "Profile updated successfully."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(
                ApiResponse<ProfileResponseDto>
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
                ApiResponse<ProfileResponseDto>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Failed to update profile."));
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