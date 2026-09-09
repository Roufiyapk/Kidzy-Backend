using Kidzy.Domain.Entities;

namespace Kidzy.Application.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}