using Kidzy.Application.DTOs.Categories;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;
using Kidzy.Domain.Entities;

namespace Kidzy.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(
        ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    // GET ALL

    public async Task<List<CategoryResponseDto>>
        GetAllAsync()
    {
        var categories =
            await _categoryRepository.GetAllAsync();

        return categories
            .Select(MapToResponse)
            .ToList();
    }

    // GET BY ID

    public async Task<CategoryResponseDto?>
        GetByIdAsync(int id)
    {
        var category =
            await _categoryRepository.GetByIdAsync(id);

        if (category == null)
            return null;

        return MapToResponse(category);
    }

    // MAPPING

    private static CategoryResponseDto
        MapToResponse(Category category)
    {
        return new CategoryResponseDto
        {
            Id = category.Id,

            Name = category.Name,

            SubCategories = category.SubCategories
                .OrderBy(x => x.Id)
                .Select(x => x.Name)
                .ToList()
        };
    }
}