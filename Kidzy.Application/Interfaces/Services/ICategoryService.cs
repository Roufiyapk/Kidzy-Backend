using Kidzy.Application.DTOs.Categories;

namespace Kidzy.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<List<CategoryResponseDto>> GetAllAsync();

    Task<CategoryResponseDto?> GetByIdAsync(int id);
}