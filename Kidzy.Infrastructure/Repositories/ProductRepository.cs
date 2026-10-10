using Kidzy.Application.DTOs.Products;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Domain.Entities;
using Kidzy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kidzy.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET ALL PRODUCTS
    public async Task<List<Product>> GetAllAsync()
    {
        return await BaseProductQuery()
            .ToListAsync();
    }

    // GET ALL PRODUCTS WITH SIMPLE SORTING
    public async Task<List<Product>> GetAllAsync(string? sort)
    {
        IQueryable<Product> query = BaseProductQuery();

        if (string.Equals(
            sort,
            "price-low",
            StringComparison.OrdinalIgnoreCase))
        {
            query = query.OrderBy(p => p.Price);
        }
        else if (string.Equals(
            sort,
            "price-high",
            StringComparison.OrdinalIgnoreCase))
        {
            query = query.OrderByDescending(p => p.Price);
        }

        return await query.ToListAsync();
    }

    // GET PRODUCT BY ID
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await BaseProductQuery()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    // GET PRODUCTS BY CATEGORY
    public async Task<List<Product>> GetByCategoryAsync(
        int categoryId)
    {
        return await BaseProductQuery()
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();
    }

    // SEARCH PRODUCTS
    public async Task<List<Product>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new List<Product>();
        }

        string search = query.Trim();

        return await BaseProductQuery()
            .Where(p =>
                p.Name.Contains(search) ||

                (p.Description != null &&
                 p.Description.Contains(search)) ||

                (p.Category != null &&
                 p.Category.Name.Contains(search)) ||

                (p.SubCategory != null &&
                 p.SubCategory.Name.Contains(search)) ||

                (p.Gender != null &&
                 p.Gender.Contains(search)) ||

                p.Variants.Any(v =>
                    v.AgeGroup != null &&
                    v.AgeGroup.Contains(search)) ||

                p.Variants.Any(v =>
                    v.Size != null &&
                    v.Size.Contains(search)))
            .ToListAsync();
    }

    // FILTER, SORT AND PAGINATE PRODUCTS
    public async Task<(
        List<Product> Products,
        int TotalProducts,
        int TotalPages,
        int CurrentPage,
        int PageSize)> GetFilteredAsync(
            ProductFilterDto filter)
    {
        filter ??= new ProductFilterDto();

        int page = filter.Page < 1
            ? 1
            : filter.Page;

        int pageSize = filter.PageSize < 1
            ? 5
            : Math.Min(filter.PageSize, 100);

        IQueryable<Product> query = BaseProductQuery();

        // STATUS FILTER
        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            switch (filter.Status.Trim().ToLowerInvariant())
            {
                case "active":
                    query = query.Where(p => !p.Deleted);
                    break;

                case "trash":
                    query = query.Where(p => p.Deleted);
                    break;

                case "all":
                    break;

                default:
                    query = query.Where(p => !p.Deleted);
                    break;
            }
        }
        else
        {
            // By default, return active products.
            query = query.Where(p => !p.Deleted);
        }

        // SEARCH FILTER
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            string search = filter.Search.Trim();

            query = query.Where(p =>
                p.Name.Contains(search) ||

                (p.Description != null &&
                 p.Description.Contains(search)) ||

                (p.Category != null &&
                 p.Category.Name.Contains(search)) ||

                (p.SubCategory != null &&
                 p.SubCategory.Name.Contains(search)) ||

                (p.Gender != null &&
                 p.Gender.Contains(search)) ||

                p.Variants.Any(v =>
                    v.AgeGroup != null &&
                    v.AgeGroup.Contains(search)) ||

                p.Variants.Any(v =>
                    v.Size != null &&
                    v.Size.Contains(search)));
        }

        // CATEGORY ID FILTER
        if (filter.CategoryId.HasValue)
        {
            query = query.Where(p =>
                p.CategoryId == filter.CategoryId.Value);
        }

        // CATEGORY NAME FILTER
        if (!string.IsNullOrWhiteSpace(filter.CategoryName))
        {
            string categoryName =
                filter.CategoryName.Trim().ToLower();

            query = query.Where(p =>
                p.Category != null &&
                p.Category.Name != null &&
                p.Category.Name.Trim().ToLower()
                    == categoryName);
        }

        // SUBCATEGORY ID FILTER
        if (filter.SubCategoryId.HasValue)
        {
            query = query.Where(p =>
                p.SubCategoryId == filter.SubCategoryId.Value);
        }

        // SUBCATEGORY NAME FILTER
        if (!string.IsNullOrWhiteSpace(filter.SubCategoryName))
        {
            string subCategoryName =
                filter.SubCategoryName.Trim().ToLower();

            query = query.Where(p =>
                p.SubCategory != null &&
                p.SubCategory.Name != null &&
                p.SubCategory.Name.Trim().ToLower()
                    == subCategoryName);
        }

        // AGE FILTER
        if (!string.IsNullOrWhiteSpace(filter.Age))
        {
            string selectedAge = NormalizeAgeLabel(filter.Age);

            query = query.Where(p =>
                p.Variants.Any(v =>
                    v.AgeGroup != null &&
                    v.AgeGroup.Trim()
                        .Replace("–", "-")
                        .Replace("—", "-")
                        .ToLower() == selectedAge));
        }

        // GENDER FILTER
        if (!string.IsNullOrWhiteSpace(filter.Gender))
        {
            string gender = filter.Gender.Trim().ToLower();

            query = query.Where(p =>
                p.Gender != null &&
                p.Gender.Trim().ToLower() == gender);
        }

        // PRICE RANGE FILTER
        if (!string.IsNullOrWhiteSpace(filter.PriceRange))
        {
            switch (filter.PriceRange.Trim())
            {
                case "Under500":
                    query = query.Where(p =>
                        p.Price < 500);
                    break;

                case "500To1000":
                    query = query.Where(p =>
                        p.Price >= 500 &&
                        p.Price <= 1000);
                    break;

                case "1000To2000":
                    query = query.Where(p =>
                        p.Price > 1000 &&
                        p.Price <= 2000);
                    break;

                case "Above2000":
                    query = query.Where(p =>
                        p.Price > 2000);
                    break;
            }
        }

        // STOCK STATUS FILTER
        if (!string.IsNullOrWhiteSpace(filter.StockStatus))
        {
            switch (filter.StockStatus.Trim())
            {
                case "InStock":
                    query = query.Where(p =>
                        p.Variants.Any()
                            ? (p.Variants.Sum(
                                v => (int?)v.Stock) ?? 0) > 10
                            : p.Stock > 10);
                    break;

                case "LowStock":
                    query = query.Where(p =>
                        p.Variants.Any()
                            ? (p.Variants.Sum(
                                  v => (int?)v.Stock) ?? 0) > 0 &&
                              (p.Variants.Sum(
                                  v => (int?)v.Stock) ?? 0) <= 10
                            : p.Stock > 0 && p.Stock <= 10);
                    break;

                case "OutOfStock":
                    query = query.Where(p =>
                        p.Variants.Any()
                            ? (p.Variants.Sum(
                                v => (int?)v.Stock) ?? 0) <= 0
                            : p.Stock <= 0);
                    break;
            }
        }

        // BEST SELLER FILTER
        if (filter.BestSeller.HasValue)
        {
            query = query.Where(p =>
                p.BestSeller == filter.BestSeller.Value);
        }

        // NEW ARRIVAL FILTER
        if (filter.NewArrival.HasValue)
        {
            query = query.Where(p =>
                p.NewArrival == filter.NewArrival.Value);
        }

        // SORTING
        query = filter.SortBy?.Trim() switch
        {
            "PriceLowToHigh" =>
                query.OrderBy(p => p.Price),

            "PriceHighToLow" =>
                query.OrderByDescending(p => p.Price),

            "Relevance" =>
                query.OrderBy(p => p.Id),

            _ =>
                query.OrderByDescending(p => p.Id)
        };

        // COUNT FILTERED PRODUCTS
        int totalProducts = await query.CountAsync();

        // CALCULATE TOTAL PAGES
        int totalPages = (int)Math.Ceiling(
            totalProducts / (double)pageSize);

        // CORRECT PAGE IF OUT OF RANGE
        if (totalPages > 0 && page > totalPages)
        {
            page = totalPages;
        }

        // PAGINATION
        List<Product> products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (
            products,
            totalProducts,
            totalPages,
            page,
            pageSize);
    }

    // ADD PRODUCT
    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    // UPDATE PRODUCT
    public async Task UpdateAsync(Product product)
    {
        product.Category = null;
        product.SubCategory = null;

        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    // DELETE PRODUCT
    public async Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

    // QUERY PRODUCTS
    public IQueryable<Product> Query()
    {
        return BaseProductQuery();
    }

    // COMMON PRODUCT QUERY
    private IQueryable<Product> BaseProductQuery()
    {
        return _context.Products
            .Include(p => p.Category)
            .Include(p => p.SubCategory)
            .Include(p => p.Variants)
            .AsNoTracking();
    }

    // NORMALIZE AGE LABELS
    private static string NormalizeAgeLabel(string? value)
    {
        return (value ?? string.Empty)
            .Trim()
            .Replace("–", "-")
            .Replace("—", "-")
            .ToLowerInvariant();
    }
}