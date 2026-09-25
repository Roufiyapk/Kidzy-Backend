using Kidzy.Application.DTOs.Admin;
using Kidzy.Application.DTOs.Profile;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;
using Kidzy.Domain.Entities;

namespace Kidzy.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(
        IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // ADMIN - GET USERS

    public async Task<List<UserListDto>>
        GetUsersAsync()
    {
        var users =
            await _userRepository
                .GetAllAsync();

        return users
            .Select(MapToUserListDto)
            .ToList();
    }

    // ADMIN - BLOCK USER

    public async Task BlockUserAsync(int id)
    {
        var user =
            await _userRepository
                .GetByIdAsync(id);

        if (user == null)
        {
            throw new Exception(
                "User not found.");
        }

        user.IsBlocked = true;

        await _userRepository
            .SaveChangesAsync();
    }

    // ADMIN - UNBLOCK USER

    public async Task UnblockUserAsync(int id)
    {
        var user =
            await _userRepository
                .GetByIdAsync(id);

        if (user == null)
        {
            throw new Exception(
                "User not found.");
        }

        user.IsBlocked = false;

        await _userRepository
            .SaveChangesAsync();
    }

    // PROFILE - GET

    public async Task<ProfileResponseDto?>
        GetProfileAsync(int userId)
    {
        var user =
            await _userRepository
                .GetByIdAsync(userId);

        if (user == null)
        {
            return null;
        }

        return MapToProfileDto(user);
    }

    // PROFILE - UPDATE

    public async Task<ProfileResponseDto>
        UpdateProfileAsync(
            int userId,
            UpdateProfileDto dto)
    {
        var user =
            await _userRepository
                .GetByIdAsync(userId);

        if (user == null)
        {
            throw new Exception(
                "User not found.");
        }

        // Normalize email
        var email =
            dto.Email
                .Trim()
                .ToLowerInvariant();

        // Check duplicate email
        var emailExists =
            await _userRepository
                .ExistsByEmailAsync(
                    email,
                    userId);

        if (emailExists)
        {
            throw new Exception(
                "Email is already used by another account.");
        }

        // Update name
        user.Name =
            dto.Name.Trim();

        // Update email
        user.Email =
            email;

        // Update phone
        user.Phone =
            string.IsNullOrWhiteSpace(dto.Phone)
                ? null
                : dto.Phone.Trim();

        // Update saved address
        user.Address =
            string.IsNullOrWhiteSpace(dto.Address)
                ? null
                : dto.Address.Trim();

        // Update pincode
        user.Pincode =
            string.IsNullOrWhiteSpace(dto.Pincode)
                ? null
                : dto.Pincode.Trim();

        await _userRepository
            .SaveChangesAsync();

        return MapToProfileDto(user);
    }

    // ADMIN DTO MAPPING

    private static UserListDto
        MapToUserListDto(User user)
    {
        return new UserListDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),
            IsBlocked = user.IsBlocked
        };
    }

    // PROFILE DTO MAPPING

    private static ProfileResponseDto
        MapToProfileDto(User user)
    {
        return new ProfileResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            Address = user.Address,
            Pincode = user.Pincode
        };
    }
}