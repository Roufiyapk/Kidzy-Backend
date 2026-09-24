using Kidzy.Application.DTOs.Admin;

namespace Kidzy.Application.Interfaces.Services;

public interface IUserService
{
    Task<IEnumerable<UserListDto>> GetUsersAsync();

    Task BlockUserAsync(int id);

    Task UnblockUserAsync(int id);
}