using Kidzy.Application.DTOs.Products;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;
using Kidzy.Domain.Entities;

namespace Kidzy.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    private readonly ICategoryRepository _categoryRepository;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<List<ProductResponseDto>>
        GetAllAsync()
    {
        var products =
            await _productRepository.GetAllAsync();

        return products
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<ProductResponseDto?>
        GetByIdAsync(int id)
    {
        var product =
            await _productRepository
                .GetByIdAsync(id);

        if (product == null)
            return null;

        return MapToResponse(product);
    }

    public async Task<List<ProductResponseDto>>
        GetByCategoryAsync(int categoryId)
    {
        var category =
            await _categoryRepository
                .GetByIdAsync(categoryId);

        if (category == null)
            return new List<ProductResponseDto>();

        var products =
            await _productRepository
                .GetByCategoryAsync(categoryId);

        return products
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<ProductResponseDto>
        CreateAsync(CreateProductDto dto)
    {
        var category =
            await _categoryRepository
                .GetByIdAsync(dto.CategoryId);

        if (category == null)
            throw new Exception(
                "Category not found.");

        var subCategory =
            category.SubCategories
                .FirstOrDefault(
                    x =>
                        x.Id ==
                        dto.SubCategoryId);

        if (subCategory == null)
        {
            throw new Exception(
                "SubCategory not found for this category.");
        }

        ValidateProduct(
            dto.Name,
            dto.Price,
            dto.Stock,
            dto.ImageUrl,
            dto.Variants);

        var product =
            new Product
            {
                Name =
                    dto.Name.Trim(),

                Description =
                    dto.Description?.Trim()
                    ?? string.Empty,

                Price =
                    dto.Price,

                Stock =
                    dto.Stock,

                ImageUrl =
                    dto.ImageUrl.Trim(),

                BestSeller =
                    dto.BestSeller,

                NewArrival =
                    dto.NewArrival,

                CategoryId =
                    dto.CategoryId,

                SubCategoryId =
                    dto.SubCategoryId
            };

        foreach (var ageGroup
                 in dto.Variants
                    ?? new List<ProductVariantDto>())
        {
            foreach (var size
                     in ageGroup.Sizes)
            {
                product.Variants.Add(
                    new ProductVariant
                    {
                        AgeGroup =
                            ageGroup.AgeGroup.Trim(),

                        Size =
                            size.Size.Trim(),

                        Stock =
                            size.Stock
                    });
            }
        }

        await _productRepository
            .AddAsync(product);

        var created =
            await _productRepository
                .GetByIdAsync(product.Id);

        if (created == null)
        {
            throw new Exception(
                "Product could not be created.");
        }

        return MapToResponse(created);
    }

    public async Task<ProductResponseDto?>
        UpdateAsync(
            int id,
            UpdateProductDto dto)
    {
        var product =
            await _productRepository
                .GetByIdAsync(id);

        if (product == null)
            return null;

        var category =
            await _categoryRepository
                .GetByIdAsync(dto.CategoryId);

        if (category == null)
        {
            throw new Exception(
                "Category not found.");
        }

        var subCategory =
            category.SubCategories
                .FirstOrDefault(
                    x =>
                        x.Id ==
                        dto.SubCategoryId);

        if (subCategory == null)
        {
            throw new Exception(
                "SubCategory not found for this category.");
        }

        ValidateProduct(
            dto.Name,
            dto.Price,
            dto.Stock,
            dto.ImageUrl,
            dto.Variants);

        product.Name =
            dto.Name.Trim();

        product.Description =
            dto.Description?.Trim()
            ?? string.Empty;

        product.Price =
            dto.Price;

        product.Stock =
            dto.Stock;

        product.ImageUrl =
            dto.ImageUrl.Trim();

        product.BestSeller =
            dto.BestSeller;

        product.NewArrival =
            dto.NewArrival;

        product.CategoryId =
            dto.CategoryId;

        product.SubCategoryId =
            dto.SubCategoryId;

        product.Variants.Clear();

        foreach (var ageGroup
                 in dto.Variants
                    ?? new List<ProductVariantDto>())
        {
            foreach (var size
                     in ageGroup.Sizes)
            {
                product.Variants.Add(
                    new ProductVariant
                    {
                        ProductId =
                            product.Id,

                        AgeGroup =
                            ageGroup.AgeGroup.Trim(),

                        Size =
                            size.Size.Trim(),

                        Stock =
                            size.Stock
                    });
            }
        }

        await _productRepository
            .UpdateAsync(product);

        var updated =
            await _productRepository
                .GetByIdAsync(product.Id);

        if (updated == null)
            return null;

        return MapToResponse(updated);
    }

    public async Task<bool>
        DeleteAsync(int id)
    {
        var product =
            await _productRepository
                .GetByIdAsync(id);

        if (product == null)
            return false;

        await _productRepository
            .DeleteAsync(product);

        return true;
    }

    private static void ValidateProduct(
        string name,
        decimal price,
        int stock,
        string imageUrl,
        List<ProductVariantDto>? variants)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exception(
                "Product name is required.");

        if (price <= 0)
            throw new Exception(
                "Price must be greater than zero.");

        if (stock < 0)
            throw new Exception(
                "Stock cannot be negative.");

        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new Exception(
                "Image URL is required.");

        if (variants == null)
            return;

        foreach (var ageGroup in variants)
        {
            if (string.IsNullOrWhiteSpace(
                    ageGroup.AgeGroup))
            {
                throw new Exception(
                    "Age group is required.");
            }

            foreach (var size in ageGroup.Sizes)
            {
                if (string.IsNullOrWhiteSpace(
                        size.Size))
                {
                    throw new Exception(
                        "Size is required.");
                }

                if (size.Stock < 0)
                {
                    throw new Exception(
                        "Stock cannot be negative.");
                }
            }
        }
    }

    private static ProductResponseDto
        MapToResponse(Product product)
    {
        return new ProductResponseDto
        {
            Id =
                product.Id,

            Name =
                product.Name,

            Description =
                product.Description,

            Price =
                product.Price,

            Stock =
                product.Stock,

            ImageUrl =
                product.ImageUrl,

            BestSeller =
                product.BestSeller,

            NewArrival =
                product.NewArrival,

            CategoryId =
                product.CategoryId,

            CategoryName =
                product.Category?.Name
                ?? string.Empty,

            SubCategory =
                product.SubCategory?.Name
                ?? string.Empty,

            Variants =
                product.Variants
                    .GroupBy(v => v.AgeGroup)
                    .Select(
                        group =>
                            new ProductVariantDto
                            {
                                AgeGroup =
                                    group.Key
                                    ?? string.Empty,

                                Sizes =
                                    group
                                        .Select(
                                            v =>
                                                new SizeStockDto
                                                {
                                                    Id =
                                                        v.Id,

                                                    Size =
                                                        v.Size
                                                        ?? string.Empty,

                                                    Stock =
                                                        v.Stock
                                                })
                                        .ToList()
                            })
                    .ToList()
        };
    }
}