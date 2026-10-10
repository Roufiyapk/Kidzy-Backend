using Kidzy.Application.DTOs.Profile;
using Kidzy.Application.DTOs.User;

namespace Kidzy.Application.Interfaces.Services;

public interface IUserService
{
    // ADMIN
    Task<List<UserListDto>> GetUsersAsync(
        string? search,
        string? status,
        int page,
        int pageSize);

    Task BlockUserAsync(int id);

    Task UnblockUserAsync(int id);

    // PROFILE
    Task<ProfileResponseDto?> GetProfileAsync(
        int userId);

    Task<ProfileResponseDto> UpdateProfileAsync(
        int userId,
        UpdateProfileDto dto);
}