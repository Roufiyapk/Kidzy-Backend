using System.Net;
using Kidzy.API.Contracts;
using Kidzy.Application.DTOs.Products;
using Kidzy.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kidzy.API.Controllers.User;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;

    public ProductController(IProductService service)
    {
        _service = service;
    }

    // Get all products

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var products =
                await _service.GetAllAsync();

            return Ok(
                ApiResponse<List<ProductResponseDto>>.Success(
                    products,
                    "Products retrieved successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<List<ProductResponseDto>>.Fail(
                    new List<string>
                    {
                        ex.Message
                    },
                    "Failed to retrieve products."));
        }
    }


    // Get product by ID

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var product =
                await _service.GetByIdAsync(id);

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
                    "Product retrieved successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<ProductResponseDto>.Fail(
                    new List<string>
                    {
                        ex.Message
                    },
                    "Failed to retrieve product."));
        }
    }


    // Get products by category

    [HttpGet("category/{categoryId:int}")]
    public async Task<IActionResult> GetByCategory(
        int categoryId)
    {
        try
        {
            var products =
                await _service
                    .GetByCategoryAsync(categoryId);

            return Ok(
                ApiResponse<List<ProductResponseDto>>.Success(
                    products,
                    "Products retrieved successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<List<ProductResponseDto>>.Fail(
                    new List<string>
                    {
                        ex.Message
                    },
                    "Failed to retrieve products."));
        }
    }
}