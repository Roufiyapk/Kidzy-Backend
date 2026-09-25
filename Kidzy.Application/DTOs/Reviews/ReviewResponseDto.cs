namespace Kidzy.Application.DTOs.Reviews;

public class ReviewResponseDto
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public int Rating { get; set; }

    public string? Title { get; set; }

    public string Comment { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public string ProductImage { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}