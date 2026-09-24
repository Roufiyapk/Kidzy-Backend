using System.Net;
using FluentValidation;
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
    private readonly IValidator<RegisterDto> _registerValidator;
    private readonly IValidator<LoginDto> _loginValidator;

    public AuthController(
        IAuthService authService,
        IValidator<RegisterDto> registerValidator,
        IValidator<LoginDto> loginValidator)
    {
        _authService = authService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterDto dto)
    {
        var validationResult =
            await _registerValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            return BadRequest(
                ApiResponse<AuthResponseDto>.Fail(
                    validationResult.Errors
                        .Select(x => x.ErrorMessage)
                        .ToList(),
                    "Validation failed."));
        }

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
                    new List<string> { ex.Message },
                    "Registration failed."));
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginDto dto)
    {
        var validationResult =
            await _loginValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            return BadRequest(
                ApiResponse<AuthResponseDto>.Fail(
                    validationResult.Errors
                        .Select(x => x.ErrorMessage)
                        .ToList(),
                    "Validation failed."));
        }

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
                    new List<string> { ex.Message },
                    "Login failed.",
                    HttpStatusCode.Unauthorized));
        }
    }
}