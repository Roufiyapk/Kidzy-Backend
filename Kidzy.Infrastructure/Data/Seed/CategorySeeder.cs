using Kidzy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kidzy.Infrastructure.Data.Seed;

public static class CategorySeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context)
    {
        if (await context.Categories.AnyAsync())
            return;

        var categories = new List<Category>
        {
            new Category
            {
                Name = "Girls",
                SubCategories =
                {
                    new SubCategory { Name = "Dresses" },
                    new SubCategory { Name = "Tops" },
                    new SubCategory { Name = "Bottoms" },
                    new SubCategory { Name = "Ethnic Wear" },
                    new SubCategory { Name = "Party Wear" }
                }
            },

            new Category
            {
                Name = "Boys",
                SubCategories =
                {
                    new SubCategory { Name = "T-Shirts" },
                    new SubCategory { Name = "Shirts" },
                    new SubCategory { Name = "Pants" },
                    new SubCategory { Name = "Ethnic Wear" },
                    new SubCategory { Name = "Party Wear" }
                }
            },

            new Category
            {
                Name = "Toys",
                SubCategories =
                {
                    new SubCategory { Name = "Educational Toys" },
                    new SubCategory { Name = "Soft Toys" },
                    new SubCategory { Name = "Dolls" },
                    new SubCategory { Name = "Cars & Vehicles" }
                }
            },

            new Category
            {
                Name = "Kids Accessories",
                SubCategories =
                {
                    new SubCategory { Name = "Bags" },
                    new SubCategory { Name = "Caps" },
                    new SubCategory { Name = "Hair Accessories" },
                    new SubCategory { Name = "Watches" },
                    new SubCategory { Name = "Sunglasses" }
                }
            },

            new Category
            {
                Name = "School Supplies",
                SubCategories =
                {
                    new SubCategory { Name = "School Bags" },
                    new SubCategory { Name = "Water Bottles" },
                    new SubCategory { Name = "Lunch Boxes" }
                }
            },

            new Category
            {
                Name = "Footwear",
                SubCategories =
                {
                    new SubCategory { Name = "Sneakers" },
                    new SubCategory { Name = "Sandals" },
                    new SubCategory { Name = "School Shoes" },
                    new SubCategory { Name = "Casual Shoes" },
                    new SubCategory { Name = "Boots" }
                }
            },

            new Category
            {
                Name = "Books",
                SubCategories =
                {
                    new SubCategory { Name = "Story Books" },
                    new SubCategory { Name = "Coloring Books" },
                    new SubCategory { Name = "Activity Books" },
                    new SubCategory { Name = "Educational Books" }
                }
            },

            new Category
            {
                Name = "Kids Care",
                SubCategories =
                {
                    new SubCategory { Name = "Bath & Body" },
                    new SubCategory { Name = "Personal Care" }
                }
            },

            new Category
            {
                Name = "Activities",
                SubCategories =
                {
                    new SubCategory { Name = "Arts & Crafts" },
                    new SubCategory { Name = "DIY Kits" },
                    new SubCategory { Name = "Puzzles" }
                }
            }
        };

        await context.Categories
            .AddRangeAsync(categories);

        await context.SaveChangesAsync();
    }
}