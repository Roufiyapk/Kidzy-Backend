using Kidzy.Application.DTOs.Category;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;

namespace Kidzy.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(
            ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<CategoryResponseDto>> GetAllAsync()
        {
            var categories =
                await _categoryRepository.GetAllAsync();

            return categories.Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();
        }

        public async Task<CategoryResponseDto?> GetByIdAsync(int id)
        {
            var category =
                await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return null;
            }

            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }
    }
}