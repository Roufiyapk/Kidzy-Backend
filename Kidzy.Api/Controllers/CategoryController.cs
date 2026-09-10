using Kidzy.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kidzy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(
            ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories =
                await _categoryService.GetAllAsync();

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category =
                await _categoryService.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Category not found"
                });
            }

            return Ok(category);
        }
    }
}