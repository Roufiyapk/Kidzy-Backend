using System.Net;
using Kidzy.API.Contracts;
using Kidzy.Application.DTOs.Products;
using Kidzy.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kidzy.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;

    public ProductController(
        IProductService service)
    {
        _service = service;
    }


    // USER - GET ALL PRODUCTS
    // OPTIONAL PRICE SORTING

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? sort)
    {
        try
        {
            var products =
                await _service.GetAllAsync(sort);

            return Ok(
                ApiResponse<List<ProductResponseDto>>
                    .Success(
                        products,
                        "Products retrieved successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<List<ProductResponseDto>>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Failed to retrieve products."));
        }
    }


    // USER - GET PRODUCT BY ID

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id)
    {
        try
        {
            var product =
                await _service.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound(
                    ApiResponse<ProductResponseDto>
                        .Fail(
                            new List<string>
                            {
                                "Product not found."
                            },
                            "Product not found.",
                            HttpStatusCode.NotFound));
            }

            return Ok(
                ApiResponse<ProductResponseDto>
                    .Success(
                        product,
                        "Product retrieved successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<ProductResponseDto>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Failed to retrieve product."));
        }
    }


    // USER - GET PRODUCTS BY CATEGORY

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
                ApiResponse<List<ProductResponseDto>>
                    .Success(
                        products,
                        "Products retrieved successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<List<ProductResponseDto>>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Failed to retrieve products."));
        }
    }


    // USER - SEARCH PRODUCTS

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest(
                ApiResponse<List<ProductResponseDto>>
                    .Fail(
                        new List<string>
                        {
                            "Search query is required."
                        },
                        "Search failed."));
        }

        try
        {
            var products =
                await _service.SearchAsync(query);

            return Ok(
                ApiResponse<List<ProductResponseDto>>
                    .Success(
                        products,
                        "Search results retrieved successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<List<ProductResponseDto>>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Failed to search products."));
        }
    }


    
    // ADMIN - CREATE PRODUCT

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create(
        [FromForm] CreateProductDto dto)
    {
        try
        {
            var product =
                await _service.CreateAsync(dto);

            return StatusCode(
                (int)HttpStatusCode.Created,

                ApiResponse<ProductResponseDto>
                    .Success(
                        product,
                        "Product created successfully.",
                        HttpStatusCode.Created));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<ProductResponseDto>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Failed to create product."));
        }
    }


    // ADMIN - UPDATE PRODUCT

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(
        int id,
        [FromForm] UpdateProductDto dto)
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
                    ApiResponse<ProductResponseDto>
                        .Fail(
                            new List<string>
                            {
                                "Product not found."
                            },
                            "Product not found.",
                            HttpStatusCode.NotFound));
            }

            return Ok(
                ApiResponse<ProductResponseDto>
                    .Success(
                        product,
                        "Product updated successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<ProductResponseDto>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Failed to update product."));
        }
    }


    // ADMIN - DELETE PRODUCT

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(
        int id)
    {
        try
        {
            var deleted =
                await _service.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(
                    ApiResponse<string>
                        .Fail(
                            new List<string>
                            {
                                "Product not found."
                            },
                            "Product not found.",
                            HttpStatusCode.NotFound));
            }

            return Ok(
                ApiResponse<string>
                    .Success(
                        string.Empty,
                        "Product deleted successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<string>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Failed to delete product."));
        }
    }
}