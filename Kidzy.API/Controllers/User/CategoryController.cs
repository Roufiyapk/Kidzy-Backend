using System.Net;
using Kidzy.API.Contracts;
using Kidzy.Application.DTOs.Categories;
using Kidzy.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kidzy.API.Controllers.User;

[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoryController(
        ICategoryService service)
    {
        _service = service;
    }

    // Get all categories

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var categories =
                await _service.GetAllAsync();

            return Ok(
                ApiResponse<List<CategoryResponseDto>>.Success(
                    categories,
                    "Categories retrieved successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<List<CategoryResponseDto>>.Fail(
                    new List<string>
                    {
                        ex.Message
                    },
                    "Failed to retrieve categories."));
        }
    }


    // Get category by ID

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id)
    {
        try
        {
            var category =
                await _service.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound(
                    ApiResponse<CategoryResponseDto>.Fail(
                        new List<string>
                        {
                            "Category not found."
                        },
                        "Category not found.",
                        HttpStatusCode.NotFound));
            }

            return Ok(
                ApiResponse<CategoryResponseDto>.Success(
                    category,
                    "Category retrieved successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<CategoryResponseDto>.Fail(
                    new List<string>
                    {
                        ex.Message
                    },
                    "Failed to retrieve category."));
        }
    }
}