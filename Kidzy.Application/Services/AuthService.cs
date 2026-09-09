using Kidzy.Application.DTOs.Auth;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;
using Kidzy.Domain.Entities;

namespace Kidzy.Application.Services
{
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

        // REGISTER
        public async Task<AuthResponseDto?> RegisterAsync(
            RegisterDto registerDto)
        {
            // Check email already exists
            var existingUser =
                await _userRepository.GetByEmailAsync(
                    registerDto.Email);

            if (existingUser != null)
            {
                return null;
            }

            // Create user
            var user = new User
            {
                Name = registerDto.Name,

                Email = registerDto.Email,

                PasswordHash =
                    _passwordHasher.HashPassword(
                        registerDto.Password),

                // Every normal registration is User
                Role = "User"
            };

            // Save user
            var createdUser =
                await _userRepository.CreateAsync(user);

            // Generate JWT
            var token =
                _jwtService.GenerateToken(createdUser);

            // Return response
            return new AuthResponseDto
            {
                UserId = createdUser.Id,

                Name = createdUser.Name,

                Email = createdUser.Email,

                Role = createdUser.Role,

                Token = token
            };
        }

        // LOGIN
        public async Task<AuthResponseDto?> LoginAsync(
            LoginDto loginDto)
        {
            // Find user by email
            var user =
                await _userRepository.GetByEmailAsync(
                    loginDto.Email);

            if (user == null)
            {
                return null;
            }

            // Verify password
            var passwordValid =
                _passwordHasher.VerifyPassword(
                    loginDto.Password,
                    user.PasswordHash);

            if (!passwordValid)
            {
                return null;
            }

            // Generate JWT
            var token =
                _jwtService.GenerateToken(user);

            // Return response
            return new AuthResponseDto
            {
                UserId = user.Id,

                Name = user.Name,

                Email = user.Email,

                Role = user.Role,

                Token = token
            };
        }
    }
}