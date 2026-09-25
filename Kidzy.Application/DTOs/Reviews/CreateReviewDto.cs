namespace Kidzy.Application.DTOs.Reviews;

public class CreateReviewDto
{
    public int ProductId { get; set; }

    public int Rating { get; set; }

    public string? Title { get; set; }

    public string Comment { get; set; } = string.Empty;
}