using Kidzy.Application.DTOs.Products;

namespace Kidzy.Application.Interfaces.Services;

public interface IProductService
{
    Task<List<ProductResponseDto>> GetAllAsync();

    Task<ProductPagedResponseDto> GetAllAsync(
        ProductFilterDto filter);

    Task<ProductResponseDto?> GetByIdAsync(
        int id);

    Task<List<ProductResponseDto>> GetByCategoryAsync(
        int categoryId);

    Task<List<ProductResponseDto>> SearchAsync(
        string query);

    Task<ProductResponseDto> CreateAsync(
        CreateProductDto dto);

    Task<ProductResponseDto?> UpdateAsync(
        int id,
        UpdateProductDto dto);

    Task<bool> MoveToTrashAsync(
        int id);

    Task<bool> RestoreAsync(
        int id);

    Task<bool> DeleteAsync(
        int id);
}