using Kidzy.Application.DTOs.Admin;
using Kidzy.Application.DTOs.Profile;

namespace Kidzy.Application.Interfaces.Services;

public interface IUserService
{
    // ADMIN

    Task<List<UserListDto>> GetUsersAsync();

    Task BlockUserAsync(int id);

    Task UnblockUserAsync(int id);

    // PROFILE

    Task<ProfileResponseDto?> GetProfileAsync(
        int userId);

    Task<ProfileResponseDto> UpdateProfileAsync(
        int userId,
        UpdateProfileDto dto);
}