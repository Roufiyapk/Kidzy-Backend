using Kidzy.Application.DTOs.Products;

namespace Kidzy.Application.Interfaces.Services;

public interface IProductService
{

    Task<List<ProductResponseDto>>
        GetAllAsync();



    Task<List<ProductResponseDto>>
        GetAllAsync(
            ProductFilterDto filter);



    Task<ProductResponseDto?>
        GetByIdAsync(
            int id);



    Task<List<ProductResponseDto>>
        GetByCategoryAsync(
            int categoryId);



    Task<List<ProductResponseDto>>
        SearchAsync(
            string query);



    Task<ProductResponseDto>
        CreateAsync(
            CreateProductDto dto);



    Task<ProductResponseDto?>
        UpdateAsync(
            int id,
            UpdateProductDto dto);



    Task<bool>
        DeleteAsync(
            int id);
}