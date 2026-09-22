using Kidzy.Domain.Entities;

namespace Kidzy.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}