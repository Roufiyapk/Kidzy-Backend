using Kidzy.Application.DTOs.Product;

namespace Kidzy.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<List<ProductResponseDto>> GetAllAsync();

        Task<ProductResponseDto?> GetByIdAsync(int id);

        Task<ProductResponseDto?> CreateAsync(
            ProductCreateDto dto);

        Task<bool> DeleteAsync(int id);
    }
}