using Kidzy.Application.DTOs.Category;

namespace Kidzy.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryResponseDto>> GetAllAsync();

        Task<CategoryResponseDto?> GetByIdAsync(int id);
    }
}