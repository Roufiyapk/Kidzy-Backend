using System.Net;
using Kidzy.API.Contracts;
using Kidzy.Application.DTOs.Auth;
using Kidzy.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kidzy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(
        IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterDto dto)
    {
        try
        {
            var result =
                await _authService.RegisterAsync(dto);

            return StatusCode(
                (int)HttpStatusCode.Created,

                ApiResponse<AuthResponseDto>.Success(
                    result,
                    "Registration successful.",
                    HttpStatusCode.Created));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<AuthResponseDto>.Fail(
                    new List<string>
                    {
                        ex.Message
                    },
                    "Registration failed."));
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginDto dto)
    {
        try
        {
            var result =
                await _authService.LoginAsync(dto);

            return Ok(
                ApiResponse<AuthResponseDto>.Success(
                    result,
                    "Login successful."));
        }
        catch (Exception ex)
        {
            return Unauthorized(
                ApiResponse<AuthResponseDto>.Fail(
                    new List<string>
                    {
                        ex.Message
                    },
                    "Login failed.",
                    HttpStatusCode.Unauthorized));
        }
    }
}