using Kidzy.Domain.Enums;

namespace Kidzy.Domain.Entities;

public class User
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.User;

    public bool IsBlocked { get; set; } = false;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Pincode { get; set; }
}