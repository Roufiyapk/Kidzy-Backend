using System.Text.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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


    // GET PRODUCTS WITH FILTERING, SORTING AND PAGINATION
    public async Task<ProductPagedResponseDto> GetAllAsync(ProductFilterDto filter)
    {
        filter ??= new ProductFilterDto();

        // Validate and normalize request values in the service layer.
        NormalizeAndValidateProductFilters(filter);

        // Database filtering, sorting and pagination are handled by the repository.
        var result = await _productRepository.GetFilteredAsync(filter);

        return new ProductPagedResponseDto
        {
            Products = result.Products.Select(MapToResponse).ToList(),
            CurrentPage = result.CurrentPage,
            PageSize = result.PageSize,
            TotalProducts = result.TotalProducts,
            TotalPages = result.TotalPages
        };
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


    // GET PRODUCTS BY CATEGORY

    public async Task<List<ProductResponseDto>>
        GetByCategoryAsync(
            int categoryId)
    {
        var category =
            await _categoryRepository
                .GetByIdAsync(
                    categoryId);

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
        // CHECK DUPLICATE PRODUCT NAME
        var normalizedName = dto.Name?.Trim();

        if (!string.IsNullOrWhiteSpace(normalizedName))
        {
            var duplicateExists = await _productRepository
                .Query()
                .AnyAsync(p =>
                    p.Name.ToLower() == normalizedName.ToLower());

            if (duplicateExists)
            {
                throw new InvalidOperationException(
                    "A product with this name already exists.");
            }
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
        var hasSubCategories =
            category.SubCategories != null &&
            category.SubCategories.Any();

        if (hasSubCategories)
        {
            if (!dto.SubCategoryId.HasValue)
            {
                throw new Exception(
                    "Please select a subcategory for this category.");
            }

            var subCategory =
                category.SubCategories!
                    .FirstOrDefault(
                        x => x.Id == dto.SubCategoryId.Value);

            if (subCategory == null)
            {
                throw new Exception(
                    "SubCategory not found for this category.");
            }
        }
        else if (dto.SubCategoryId.HasValue)
        {
            throw new Exception(
                "This category does not have subcategories. Do not select a subcategory.");
        }


        // VALIDATE PRODUCT

        ValidateProduct(
            dto.Name,
            dto.Price,
            dto.Stock,
            dto.Images,
            dto.Variants);


        // UPLOAD ALL IMAGES TO CLOUDINARY

        var imageUrls =
            new List<string>();

        foreach (var image in dto.Images)
        {
            var uploadedUrl =
                await _cloudinaryService
                    .UploadImageAsync(
                        image);

            if (!string.IsNullOrWhiteSpace(uploadedUrl))
            {
                imageUrls.Add(uploadedUrl);
            }
        }

        if (imageUrls.Count == 0)
        {
            throw new Exception(
                "Product image upload failed.");
        }



        var imageUrl = imageUrls[0];
        var storedImageUrls =
            JsonSerializer.Serialize(imageUrls);


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
                    storedImageUrls,

                BestSeller =
                    dto.BestSeller,

                NewArrival =
                    dto.NewArrival,

                CategoryId =
                    dto.CategoryId,

                SubCategoryId =
                    hasSubCategories ? dto.SubCategoryId : null,

                Deleted =
                    false
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
        var hasSubCategories =
            category.SubCategories != null &&
            category.SubCategories.Any();

        if (hasSubCategories)
        {
            if (!dto.SubCategoryId.HasValue)
            {
                throw new Exception(
                    "Please select a subcategory for this category.");
            }

            var subCategory =
                category.SubCategories!
                    .FirstOrDefault(
                        x => x.Id == dto.SubCategoryId.Value);

            if (subCategory == null)
            {
                throw new Exception(
                    "SubCategory not found for this category.");
            }
        }
        else if (dto.SubCategoryId.HasValue)
        {
            throw new Exception(
                "This category does not have subcategories. Do not select a subcategory.");
        }


        // VALIDATE PRODUCT

        ValidateProduct(
            dto.Name,
            dto.Price,
            dto.Stock,
            dto.Images,
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
            hasSubCategories ? dto.SubCategoryId : null;


        // UPDATE IMAGES

        if (dto.Images != null && dto.Images.Count > 0)
        {
            var newImageUrls =
                new List<string>();

            foreach (var image in dto.Images)
            {
                if (image == null || image.Length == 0)
                {
                    continue;
                }

                var uploadedUrl =
                    await _cloudinaryService
                        .UploadImageAsync(
                            image);

                if (!string.IsNullOrWhiteSpace(uploadedUrl))
                {
                    newImageUrls.Add(uploadedUrl);
                }
            }

            if (newImageUrls.Count == 0)
            {
                throw new Exception(
                    "Product image upload failed.");
            }

            product.ImageUrl =
                JsonSerializer.Serialize(
                    newImageUrls);
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


    // MOVE PRODUCT TO TRASH

    public async Task<bool>
        MoveToTrashAsync(
            int id)
    {
        var product =
            await _productRepository
                .GetByIdAsync(id);

        if (product == null)
        {
            return false;
        }

        product.Deleted = true;

        await _productRepository
            .UpdateAsync(product);

        return true;
    }


    // RESTORE PRODUCT


    public async Task<bool>
        RestoreAsync(
            int id)
    {
        var product =
            await _productRepository
                .GetByIdAsync(id);

        if (product == null)
        {
            return false;
        }

        product.Deleted = false;

        await _productRepository
            .UpdateAsync(product);

        return true;
    }


    // PERMANENT DELETE

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


    // FILTER VALIDATION AND NORMALIZATION
    private static void NormalizeAndValidateProductFilters(ProductFilterDto filter)
    {
        // Normalize pagination values before passing the filter to the repository.
        if (filter.Page < 1)
        {
            filter.Page = 1;
        }

        if (filter.PageSize < 1)
        {
            filter.PageSize = 5;
        }

        if (filter.PageSize > 100)
        {
            filter.PageSize = 100;
        }

        // STATUS
        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            var status = filter.Status.Trim().ToLowerInvariant();

            if (status is not ("active" or "trash" or "all"))
            {
                throw new ArgumentException("Invalid product status.");
            }

            filter.Status = status;
        }

        // AGE: allow normal hyphen, en dash and em dash in labels.
        if (!string.IsNullOrWhiteSpace(filter.Age))
        {
            var requestedAge = NormalizeAgeLabel(filter.Age);
            var validAge = ProductFilterConstants.Ages.FirstOrDefault(
                age => NormalizeAgeLabel(age) == requestedAge);

            if (validAge == null)
            {
                throw new ArgumentException("Invalid age filter.");
            }

            // Keep a canonical label. The repository also normalizes dashes when querying.
            filter.Age = validAge;
        }

        // GENDER
        if (!string.IsNullOrWhiteSpace(filter.Gender))
        {
            var requestedGender = filter.Gender.Trim();
            var validGender = ProductFilterConstants.Genders.FirstOrDefault(
                gender => string.Equals(gender, requestedGender, StringComparison.OrdinalIgnoreCase));

            if (validGender == null)
            {
                throw new ArgumentException("Invalid gender filter.");
            }

            filter.Gender = validGender;
        }

        // PRICE RANGE
        if (!string.IsNullOrWhiteSpace(filter.PriceRange))
        {
            var requestedPriceRange = filter.PriceRange.Trim();
            var validPriceRange = ProductFilterConstants.PriceRanges.FirstOrDefault(
                value => string.Equals(value, requestedPriceRange, StringComparison.OrdinalIgnoreCase));

            if (validPriceRange == null)
            {
                throw new ArgumentException("Invalid price range.");
            }

            filter.PriceRange = validPriceRange;
        }

        // STOCK STATUS
        if (!string.IsNullOrWhiteSpace(filter.StockStatus))
        {
            var requestedStockStatus = filter.StockStatus.Trim();
            var validStockStatus = ProductFilterConstants.StockStatuses.FirstOrDefault(
                value => string.Equals(value, requestedStockStatus, StringComparison.OrdinalIgnoreCase));

            if (validStockStatus == null)
            {
                throw new ArgumentException("Invalid stock status.");
            }

            filter.StockStatus = validStockStatus;
        }

        // SORTING
        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            var requestedSort = filter.SortBy.Trim();
            var validSort = ProductFilterConstants.SortOptions.FirstOrDefault(
                value => string.Equals(value, requestedSort, StringComparison.OrdinalIgnoreCase));

            if (validSort == null)
            {
                throw new ArgumentException("Invalid sorting option.");
            }

            filter.SortBy = validSort;
        }
    }

    private static string NormalizeAgeLabel(string? value)
    {
        return (value ?? string.Empty)
            .Trim()
            .Replace("–", "-")
            .Replace("—", "-")
            .ToLowerInvariant();
    }


    // VALIDATION

    private static void ValidateProduct(
        string name,
        decimal price,
        int stock,
        List<IFormFile>? images,
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


        // IMAGES

        var validImages =
            images?
                .Where(image =>
                    image != null &&
                    image.Length > 0)
                .ToList()
            ?? new List<IFormFile>();

        if (requireImage &&
            validImages.Count == 0)
        {
            throw new Exception(
                "At least one product image is required.");
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


    // READ STORED IMAGE URLS

    private static List<string> GetStoredImageUrls(
        Product product)
    {
        if (string.IsNullOrWhiteSpace(
                product.ImageUrl))
        {
            return new List<string>();
        }

        try
        {
            var urls =
                JsonSerializer.Deserialize<List<string>>(
                    product.ImageUrl);

            if (urls != null)
            {
                return urls
                    .Where(url =>
                        !string.IsNullOrWhiteSpace(url))
                    .ToList();
            }
        }
        catch (JsonException)
        {
        }

        return new List<string>
        {
            product.ImageUrl
        };
    }


    // MAP ENTITY TO RESPONSE DTO

    private static ProductResponseDto
        MapToResponse(
            Product product)
    {
        var imageUrls =
            GetStoredImageUrls(product);

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
                imageUrls
                    .FirstOrDefault()
                ?? string.Empty,

            Images =
                imageUrls,

            BestSeller =
                product.BestSeller,

            NewArrival =
                product.NewArrival,

            Gender =
                product.Gender,

            Deleted =
                product.Deleted,

            CategoryId =
                product.CategoryId,

            CategoryName =
                product.Category?.Name
                ?? string.Empty,

            SubCategory =
                product.SubCategory?.Name
                ?? string.Empty,

            SubCategoryId =
                product.SubCategoryId,

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