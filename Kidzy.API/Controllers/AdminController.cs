using Kidzy.API.Contracts;
using Kidzy.Application.DTOs.Admin;
using Kidzy.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kidzy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(
        IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var users =
                await _adminService.GetUsersAsync();

            return Ok(
                ApiResponse<IEnumerable<UserListDto>>
                    .Success(
                        users,
                        "Users retrieved successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<IEnumerable<UserListDto>>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Failed to retrieve users."));
        }
    }

    [HttpPut("users/{id}/block")]
    public async Task<IActionResult> BlockUser(int id)
    {
        try
        {
            await _adminService.BlockUserAsync(id);

            return Ok(
                ApiResponse<string>.Success(
                    "User blocked successfully.",
                    "Success"));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<string>.Fail(
                    new List<string>
                    {
                        ex.Message
                    },
                    "Failed to block user."));
        }
    }

    [HttpPut("users/{id}/unblock")]
    public async Task<IActionResult> UnblockUser(int id)
    {
        try
        {
            await _adminService.UnblockUserAsync(id);

            return Ok(
                ApiResponse<string>.Success(
                    "User unblocked successfully.",
                    "Success"));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<string>.Fail(
                    new List<string>
                    {
                        ex.Message
                    },
                    "Failed to unblock user."));
        }
    }
}