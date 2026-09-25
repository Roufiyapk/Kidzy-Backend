namespace Kidzy.Application.DTOs.Profile;

public class UpdateProfileDto
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Pincode { get; set; }
}