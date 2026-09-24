using System.Net;
using Kidzy.API.Contracts;
using Kidzy.Application.DTOs.Products;
using Kidzy.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kidzy.API.Controllers.Admin;

[ApiController]
[Route("api/admin/products")]
[Authorize(Roles = "Admin")]
public class AdminProductController : ControllerBase
{
    private readonly IProductService _service;

    public AdminProductController(IProductService service)
    {
        _service = service;
    }


    // Create product

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductDto dto)
    {
        try
        {
            var product =
                await _service.CreateAsync(dto);

            return StatusCode(
                (int)HttpStatusCode.Created,
                ApiResponse<ProductResponseDto>.Success(
                    product,
                    "Product created successfully.",
                    HttpStatusCode.Created));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<ProductResponseDto>.Fail(
                    new List<string>
                    {
                        ex.Message
                    },
                    "Failed to create product."));
        }
    }


    // Update product

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateProductDto dto)
    {
        try
        {
            var product =
                await _service.UpdateAsync(
                    id,
                    dto);

            if (product == null)
            {
                return NotFound(
                    ApiResponse<ProductResponseDto>.Fail(
                        new List<string>
                        {
                            "Product not found."
                        },
                        "Product not found.",
                        HttpStatusCode.NotFound));
            }

            return Ok(
                ApiResponse<ProductResponseDto>.Success(
                    product,
                    "Product updated successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<ProductResponseDto>.Fail(
                    new List<string>
                    {
                        ex.Message
                    },
                    "Failed to update product."));
        }
    }


    // Delete product

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted =
                await _service.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(
                    ApiResponse<string>.Fail(
                        new List<string>
                        {
                            "Product not found."
                        },
                        "Product not found.",
                        HttpStatusCode.NotFound));
            }

            return Ok(
                ApiResponse<string>.Success(
                    string.Empty,
                    "Product deleted successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<string>.Fail(
                    new List<string>
                    {
                        ex.Message
                    },
                    "Failed to delete product."));
        }
    }
}