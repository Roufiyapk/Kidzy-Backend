using Kidzy.Application.DTOs.Admin;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;
using Kidzy.Domain.Enums;

namespace Kidzy.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // Get all normal users
    // Admin will NOT be included
    public async Task<IEnumerable<UserListDto>> GetUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users
            .Where(user => user.Role != UserRole.Admin)
            .Select(user => new UserListDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString(),
                IsBlocked = user.IsBlocked
            });
    }

    // Block user
    public async Task BlockUserAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
        {
            throw new Exception("User not found.");
        }

        // Admin cannot be blocked
        if (user.Role == UserRole.Admin)
        {
            throw new Exception(
                "Admin account cannot be blocked.");
        }

        user.IsBlocked = true;

        await _userRepository.UpdateAsync(user);
    }

    // Unblock user
    public async Task UnblockUserAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
        {
            throw new Exception("User not found.");
        }

        // Admin cannot be unblocked/changed
        if (user.Role == UserRole.Admin)
        {
            throw new Exception(
                "Admin account cannot be modified.");
        }

        user.IsBlocked = false;

        await _userRepository.UpdateAsync(user);
    }
}