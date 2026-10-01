using Microsoft.AspNetCore.Http;

using Kidzy.Application.Constants;
using Kidzy.Application.DTOs.Products;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;

using Kidzy.Domain.Entities;

namespace Kidzy.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository
        _productRepository;

    private readonly ICategoryRepository
        _categoryRepository;

    private readonly ICloudinaryService
        _cloudinaryService;


    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ICloudinaryService cloudinaryService)
    {
        _productRepository =
            productRepository;

        _categoryRepository =
            categoryRepository;

        _cloudinaryService =
            cloudinaryService;
    }


    // GET ALL PRODUCTS

    public async Task<List<ProductResponseDto>>
        GetAllAsync()
    {
        var products =
            await _productRepository
                .GetAllAsync();

        return products
            .Select(MapToResponse)
            .ToList();
    }


    // GET PRODUCTS WITH FILTERS + SORTING

    public async Task<List<ProductResponseDto>>
        GetAllAsync(
            ProductFilterDto filter)
    {
        var products =
            await _productRepository
                .GetAllAsync();


        // SUBCATEGORY FILTER

        if (filter.SubCategoryId.HasValue)
        {
            products =
                products
                    .Where(p =>
                        p.SubCategoryId ==
                        filter.SubCategoryId.Value)
                    .ToList();
        }


        // AGE FILTER

        if (!string.IsNullOrWhiteSpace(
                filter.Age))
        {
            if (!ProductFilterConstants.Ages
                .Contains(filter.Age))
            {
                throw new Exception(
                    "Invalid age filter.");
            }


            products =
                products
                    .Where(p =>
                        p.Variants.Any(v =>
                            v.AgeGroup ==
                            filter.Age))
                    .ToList();
        }


        // GENDER FILTER
        

        if (!string.IsNullOrWhiteSpace(
                filter.Gender))
        {
            if (!ProductFilterConstants.Genders
                .Contains(filter.Gender))
            {
                throw new Exception(
                    "Invalid gender filter.");
            }


            products =
                products
                    .Where(p =>
                        p.Category != null &&
                        p.Category.Name
                            .Equals(
                                filter.Gender,
                                StringComparison
                                    .OrdinalIgnoreCase))
                    .ToList();
        }


        // PRICE FILTER

        if (!string.IsNullOrWhiteSpace(
                filter.PriceRange))
        {
            if (!ProductFilterConstants.PriceRanges
                .Contains(filter.PriceRange))
            {
                throw new Exception(
                    "Invalid price range.");
            }


            products =
                filter.PriceRange switch
                {
                    "Under500" =>
                        products
                            .Where(p =>
                                p.Price < 500)
                            .ToList(),


                    "500To1000" =>
                        products
                            .Where(p =>
                                p.Price >= 500 &&
                                p.Price <= 1000)
                            .ToList(),


                    "1000To2000" =>
                        products
                            .Where(p =>
                                p.Price > 1000 &&
                                p.Price <= 2000)
                            .ToList(),


                    "Above2000" =>
                        products
                            .Where(p =>
                                p.Price > 2000)
                            .ToList(),


                    _ =>
                        products
                };
        }


        // SORTING

        if (!string.IsNullOrWhiteSpace(
                filter.SortBy))
        {
            if (!ProductFilterConstants.SortOptions
                .Contains(filter.SortBy))
            {
                throw new Exception(
                    "Invalid sorting option.");
            }


            products =
                filter.SortBy switch
                {
                    // Relevance
                    "Relevance" =>
                        products,


                    // Price Low -> High
                    "PriceLowToHigh" =>
                        products
                            .OrderBy(p =>
                                p.Price)
                            .ToList(),


                    // Price High -> Low
                    "PriceHighToLow" =>
                        products
                            .OrderByDescending(p =>
                                p.Price)
                            .ToList(),


                    _ =>
                        products
                };
        }


        return products
            .Select(MapToResponse)
            .ToList();
    }


    // GET PRODUCT BY ID

    public async Task<ProductResponseDto?>
        GetByIdAsync(
            int id)
    {
        var product =
            await _productRepository
                .GetByIdAsync(id);

        if (product == null)
        {
            return null;
        }

        return MapToResponse(product);
    }


    // GET BY CATEGORY

    public async Task<List<ProductResponseDto>>
        GetByCategoryAsync(
            int categoryId)
    {
        var category =
            await _categoryRepository
                .GetByIdAsync(categoryId);

        if (category == null)
        {
            return new List<ProductResponseDto>();
        }


        var products =
            await _productRepository
                .GetByCategoryAsync(
                    categoryId);


        return products
            .Select(MapToResponse)
            .ToList();
    }


    // SEARCH PRODUCTS

    public async Task<List<ProductResponseDto>>
        SearchAsync(
            string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new List<ProductResponseDto>();
        }


        var products =
            await _productRepository
                .SearchAsync(
                    query.Trim());


        return products
            .Select(MapToResponse)
            .ToList();
    }


    // CREATE PRODUCT

    public async Task<ProductResponseDto>
        CreateAsync(
            CreateProductDto dto)
    {
        // CHECK CATEGORY

        var category =
            await _categoryRepository
                .GetByIdAsync(
                    dto.CategoryId);

        if (category == null)
        {
            throw new Exception(
                "Category not found.");
        }


        // CHECK SUBCATEGORY

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


        // VALIDATE PRODUCT

        ValidateProduct(
            dto.Name,
            dto.Price,
            dto.Stock,
            dto.Image,
            dto.Variants);


        // UPLOAD IMAGE

        if (dto.Image == null)
        {
            throw new Exception(
                "Product image is required.");
        }


        var imageUrl =
            await _cloudinaryService
                .UploadImageAsync(
                    dto.Image);


        // CREATE PRODUCT

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
                    imageUrl,

                BestSeller =
                    dto.BestSeller,

                NewArrival =
                    dto.NewArrival,

                CategoryId =
                    dto.CategoryId,

                SubCategoryId =
                    dto.SubCategoryId
            };


        // ADD VARIANTS

        foreach (
            var ageGroup
            in dto.Variants)
        {
            foreach (
                var size
                in ageGroup.Sizes)
            {
                product.Variants.Add(
                    new ProductVariant
                    {
                        AgeGroup =
                            ageGroup
                                .AgeGroup
                                .Trim(),

                        Size =
                            size.Size
                                .Trim(),

                        Stock =
                            size.Stock
                    });
            }
        }


        // SAVE

        await _productRepository
            .AddAsync(product);


        // GET CREATED PRODUCT

        var created =
            await _productRepository
                .GetByIdAsync(
                    product.Id);

        if (created == null)
        {
            throw new Exception(
                "Product could not be created.");
        }


        return MapToResponse(created);
    }


    // UPDATE PRODUCT

    public async Task<ProductResponseDto?>
        UpdateAsync(
            int id,
            UpdateProductDto dto)
    {
        // GET EXISTING PRODUCT

        var product =
            await _productRepository
                .GetByIdAsync(id);

        if (product == null)
        {
            return null;
        }


        // CHECK CATEGORY

        var category =
            await _categoryRepository
                .GetByIdAsync(
                    dto.CategoryId);

        if (category == null)
        {
            throw new Exception(
                "Category not found.");
        }


        // CHECK SUBCATEGORY

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


        // VALIDATE PRODUCT

        ValidateProduct(
            dto.Name,
            dto.Price,
            dto.Stock,
            dto.Image,
            dto.Variants,
            requireImage: false);


        // UPDATE DETAILS

        product.Name =
            dto.Name.Trim();

        product.Description =
            dto.Description?.Trim()
            ?? string.Empty;

        product.Price =
            dto.Price;

        product.Stock =
            dto.Stock;

        product.BestSeller =
            dto.BestSeller;

        product.NewArrival =
            dto.NewArrival;

        product.CategoryId =
            dto.CategoryId;

        product.SubCategoryId =
            dto.SubCategoryId;


        // UPDATE IMAGE

        if (dto.Image != null)
        {
            var imageUrl =
                await _cloudinaryService
                    .UploadImageAsync(
                        dto.Image);

            product.ImageUrl =
                imageUrl;
        }


        // REMOVE OLD VARIANTS

        product.Variants.Clear();


        // ADD NEW VARIANTS

        foreach (
            var ageGroup
            in dto.Variants)
        {
            foreach (
                var size
                in ageGroup.Sizes)
            {
                product.Variants.Add(
                    new ProductVariant
                    {
                        ProductId =
                            product.Id,

                        AgeGroup =
                            ageGroup
                                .AgeGroup
                                .Trim(),

                        Size =
                            size.Size
                                .Trim(),

                        Stock =
                            size.Stock
                    });
            }
        }


        // SAVE UPDATE

        await _productRepository
            .UpdateAsync(product);


        // GET UPDATED PRODUCT

        var updated =
            await _productRepository
                .GetByIdAsync(
                    product.Id);

        if (updated == null)
        {
            return null;
        }


        return MapToResponse(updated);
    }


    // DELETE PRODUCT

    public async Task<bool>
        DeleteAsync(
            int id)
    {
        var product =
            await _productRepository
                .GetByIdAsync(id);

        if (product == null)
        {
            return false;
        }


        await _productRepository
            .DeleteAsync(product);


        return true;
    }


    // VALIDATION

    private static void ValidateProduct(
        string name,
        decimal price,
        int stock,
        IFormFile? image,
        List<CreateProductVariantDto> variants,
        bool requireImage = true)
    {
        // NAME

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception(
                "Product name is required.");
        }


        // PRICE

        if (price <= 0)
        {
            throw new Exception(
                "Price must be greater than zero.");
        }


        // STOCK

        if (stock < 0)
        {
            throw new Exception(
                "Stock cannot be negative.");
        }


        // IMAGE

        if (requireImage &&
            (image == null ||
             image.Length == 0))
        {
            throw new Exception(
                "Product image is required.");
        }


        // VARIANTS

        if (variants == null)
        {
            return;
        }


        foreach (
            var ageGroup
            in variants)
        {
            if (string.IsNullOrWhiteSpace(
                    ageGroup.AgeGroup))
            {
                throw new Exception(
                    "Age group is required.");
            }


            foreach (
                var size
                in ageGroup.Sizes)
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


    
    // MAP ENTITY TO RESPONSE DTO

    private static ProductResponseDto
        MapToResponse(
            Product product)
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
                    .GroupBy(
                        v =>
                            v.AgeGroup)
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