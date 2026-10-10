using Kidzy.API.Contracts;
using Kidzy.Application.DTOs.User;
using Kidzy.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kidzy.API.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public class UserController : ControllerBase
{
    private readonly IUserService _adminService;

    public UserController(IUserService adminService)
    {
        _adminService = adminService;
    }

    // GET: api/admin/users
   
    [HttpGet]
    public async Task<IActionResult> GetUsers(
        [FromQuery] string? search,
        [FromQuery] string? status = "all",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5)
    {
        try
        {
            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 5;
            }

            var users =
                await _adminService.GetUsersAsync(
                    search,
                    status,
                    page,
                    pageSize);

            return Ok(
                ApiResponse<IEnumerable<UserListDto>>.Success(
                    users,
                    "Users retrieved successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<IEnumerable<UserListDto>>.Fail(
                    new List<string>
                    {
                        ex.Message
                    },
                    "Failed to retrieve users."));
        }
    }

    // PUT: api/admin/users/{id}/block

    [HttpPut("{id:int}/block")]
    public async Task<IActionResult> BlockUser(int id)
    {
        try
        {
            await _adminService.BlockUserAsync(id);

            return Ok(
                ApiResponse<string>.Success(
                    string.Empty,
                    "User blocked successfully."));
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

    // PUT: api/admin/users/{id}/unblock

    [HttpPut("{id:int}/unblock")]
    public async Task<IActionResult> UnblockUser(int id)
    {
        try
        {
            await _adminService.UnblockUserAsync(id);

            return Ok(
                ApiResponse<string>.Success(
                    string.Empty,
                    "User unblocked successfully."));
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