using Kidzy.Application.DTOs.Category;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;
using Kidzy.Domain.Entities;

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

        public async Task<CategoryResponseDto> CreateAsync(
            CategoryCreateDto dto)
        {
            var category = new Category
            {
                Name = dto.Name
            };

            var createdCategory =
                await _categoryRepository.CreateAsync(category);

            return new CategoryResponseDto
            {
                Id = createdCategory.Id,
                Name = createdCategory.Name
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _categoryRepository.DeleteAsync(id);
        }
    }
}