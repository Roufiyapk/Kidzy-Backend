using System.Net;
using Kidzy.API.Contracts;
using Kidzy.Application.DTOs.Admin;
using Kidzy.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kidzy.API.Controllers.Admin;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public class AdminUserController : ControllerBase
{
    private readonly IUserService _adminService;

    public AdminUserController(IUserService adminService)
    {
        _adminService = adminService;
    }

    // GET: api/admin/users

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var users =
                await _adminService.GetUsersAsync();

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


    // PUT: api/admin/users/5/block

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


    // PUT: api/admin/users/5/unblock

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