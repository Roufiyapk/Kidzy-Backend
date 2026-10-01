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
            // GIRLS
            new Category
            {
                Name = "Girls",
                SubCategories =
                {
                    new SubCategory { Name = "Dresses" },
                    new SubCategory { Name = "Tops" },
                    new SubCategory { Name = "Bottoms" },
                    new SubCategory { Name = "Co-ord Sets" },
                    new SubCategory { Name = "Ethnic Wear" },
                    new SubCategory { Name = "Party Wear" }
                }
            },

            // BOYS
            new Category
            {
                Name = "Boys",
                SubCategories =
                {
                    new SubCategory { Name = "T-Shirts" },
                    new SubCategory { Name = "Shirts" },
                    new SubCategory { Name = "Pants" },
                    new SubCategory { Name = "Co-ord Sets" },
                    new SubCategory { Name = "Ethnic Wear" },
                    new SubCategory { Name = "Party Wear" }
                }
            },

            // TOYS
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

            // KIDS ACCESSORIES
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

            // SCHOOL SUPPLIES
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

            // FOOTWEAR
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

            // BOOKS
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

            // ACTIVITIES
            new Category
            {
                Name = "Activities",
                SubCategories =
                {
                    new SubCategory { Name = "Arts & Crafts" },
                    new SubCategory { Name = "DIY Kits" },
                    new SubCategory { Name = "Puzzles" }
                }
            },

            // KIDS CARE
            new Category
            {
                Name = "Kids Care"
            }
        };

        await context.Categories
            .AddRangeAsync(categories);

        await context.SaveChangesAsync();
    }
}