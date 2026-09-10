using Kidzy.Application.DTOs.Category;
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

        // GET: api/Category
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories =
                await _categoryService.GetAllAsync();

            return Ok(categories);
        }

        // GET: api/Category/1
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

        // POST: api/Category
        [HttpPost]
        public async Task<IActionResult> Create(
            CategoryCreateDto dto)
        {
            var category =
                await _categoryService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = category.Id },
                category);
        }

        // DELETE: api/Category/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted =
                await _categoryService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Category not found"
                });
            }

            return Ok(new
            {
                message = "Category deleted successfully"
            });
        }
    }
}