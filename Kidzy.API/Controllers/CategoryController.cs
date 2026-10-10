using Kidzy.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kidzy.API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();

        var result = categories
            .Select(category => new
            {
                id = category.Id,
                name = category.Name,
                subCategories = category.SubCategories
                    .Select(subCategory => new
                    {
                        id = subCategory.Id,
                        name = subCategory.Name
                    })
                    .ToList()
            })
            .ToList();

        return Ok(new
        {
            data = result
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);

        if (category == null)
        {
            return NotFound(new
            {
                message = "Category not found."
            });
        }

        var result = new
        {
            id = category.Id,
            name = category.Name,
            subCategories = category.SubCategories
                .Select(subCategory => new
                {
                    id = subCategory.Id,
                    name = subCategory.Name
                })
                .ToList()
        };

        return Ok(new
        {
            data = result
        });
    }
}