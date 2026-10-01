using Kidzy.Application.DTOs.Products;

namespace Kidzy.Application.Interfaces.Services;

public interface IProductService
{
    // GET ALL PRODUCTS

    Task<List<ProductResponseDto>>
        GetAllAsync();


    // GET PRODUCTS WITH FILTERS AND SORTING

    Task<List<ProductResponseDto>>
        GetAllAsync(
            ProductFilterDto filter);


    // GET PRODUCT BY ID

    Task<ProductResponseDto?>
        GetByIdAsync(
            int id);


    // GET BY CATEGORY

    Task<List<ProductResponseDto>>
        GetByCategoryAsync(
            int categoryId);


    // SEARCH

    Task<List<ProductResponseDto>>
        SearchAsync(
            string query);


    // CREATE

    Task<ProductResponseDto>
        CreateAsync(
            CreateProductDto dto);


    // UPDATE

    Task<ProductResponseDto?>
        UpdateAsync(
            int id,
            UpdateProductDto dto);


    // DELETE

    Task<bool>
        DeleteAsync(
            int id);
}