using Kidzy.Application.DTOs.Auth;
using Kidzy.Application.Interfaces;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;
using Kidzy.Domain.Entities;
using Kidzy.Domain.Enums;

namespace Kidzy.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> RegisterAsync(
        RegisterDto dto)
    {
        var email = dto.Email
            .Trim()
            .ToLowerInvariant();

        var existingUser =
            await _userRepository.GetByEmailAsync(email);

        if (existingUser != null)
        {
            throw new Exception(
                "Email already exists.");
        }

        var user = new User
        {
            Name = dto.Name.Trim(),

            Email = email,

            PasswordHash =
                _passwordHasher.HashPassword(
                    dto.Password),

            Role = UserRole.User,

            IsBlocked = false,

            Phone = null,

            Address = null,

            Pincode = null
        };

        var createdUser =
            await _userRepository.AddAsync(user);

        var token =
            _jwtService.GenerateToken(createdUser);

        return new AuthResponseDto
        {
            Id = createdUser.Id,

            Name = createdUser.Name,

            Email = createdUser.Email,

            Role = createdUser.Role.ToString(),

            Token = token
        };
    }

    public async Task<AuthResponseDto> LoginAsync(
        LoginDto dto)
    {
        var email = dto.Email
            .Trim()
            .ToLowerInvariant();

        var user =
            await _userRepository.GetByEmailAsync(email);

        if (user == null)
        {
            throw new Exception(
                "Invalid email or password.");
        }

        if (user.IsBlocked)
        {
            throw new Exception(
                "Your account has been blocked.");
        }

        var passwordValid =
            _passwordHasher.VerifyPassword(
                dto.Password,
                user.PasswordHash);

        if (!passwordValid)
        {
            throw new Exception(
                "Invalid email or password.");
        }

        var token =
            _jwtService.GenerateToken(user);

        return new AuthResponseDto
        {
            Id = user.Id,

            Name = user.Name,

            Email = user.Email,

            Role = user.Role.ToString(),

            Token = token
        };
    }
}