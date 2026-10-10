using Kidzy.Application.DTOs.Profile;
using Kidzy.Application.DTOs.User;
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
    // Search, status filter, sorting and pagination
    // are handled by the repository/database.

    public async Task<List<UserListDto>> GetUsersAsync(
        string? search,
        string? status,
        int page,
        int pageSize)
    {
        if (page < 1)
            page = 1;

        if (pageSize < 1)
            pageSize = 5;

        return await _userRepository.GetUsersAsync(
            search,
            status,
            page,
            pageSize);
    }

    // ADMIN - BLOCK USER

    public async Task BlockUserAsync(int id)
    {
        var user =
            await _userRepository.GetByIdAsync(id);

        if (user == null)
            throw new Exception("User not found.");

        user.IsBlocked = true;

        await _userRepository.SaveChangesAsync();
    }

    // ADMIN - UNBLOCK USER

    public async Task UnblockUserAsync(int id)
    {
        var user =
            await _userRepository.GetByIdAsync(id);

        if (user == null)
            throw new Exception("User not found.");

        user.IsBlocked = false;

        await _userRepository.SaveChangesAsync();
    }

    // PROFILE - GET

    public async Task<ProfileResponseDto?> GetProfileAsync(
        int userId)
    {
        var user =
            await _userRepository.GetByIdAsync(userId);

        if (user == null)
            return null;

        return MapToProfileDto(user);
    }

    // PROFILE - UPDATE

    public async Task<ProfileResponseDto> UpdateProfileAsync(
        int userId,
        UpdateProfileDto dto)
    {
        var user =
            await _userRepository.GetByIdAsync(userId);

        if (user == null)
            throw new Exception("User not found.");

        var email =
            dto.Email.Trim().ToLowerInvariant();

        var emailExists =
            await _userRepository.ExistsByEmailAsync(
                email,
                userId);

        if (emailExists)
        {
            throw new Exception(
                "Email is already used by another account.");
        }

        user.Name = dto.Name.Trim();

        user.Email = email;

        user.Phone =
            string.IsNullOrWhiteSpace(dto.Phone)
                ? null
                : dto.Phone.Trim();

        user.Address =
            string.IsNullOrWhiteSpace(dto.Address)
                ? null
                : dto.Address.Trim();

        user.Pincode =
            string.IsNullOrWhiteSpace(dto.Pincode)
                ? null
                : dto.Pincode.Trim();

        await _userRepository.SaveChangesAsync();

        return MapToProfileDto(user);
    }

    // MAP USER TO PROFILE DTO

    private static ProfileResponseDto MapToProfileDto(
        User user)
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