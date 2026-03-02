namespace Francesco.Recipes.World.Repositories.Category
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Francesco.Recipes.World.Data;
    using Francesco.Recipes.World.Models;
    using Francesco.Recipes.World.Models.BackendModels.Category;
    using Francesco.Recipes.World.Models.BackendModels.Recipe;
    using Francesco.Recipes.World.Views.Category;
    using Microsoft.EntityFrameworkCore;

    public class CategoryRepository : ICategoryRepository
    {
        private readonly FrancescosRecipesWorldDbContext _context;

        public CategoryRepository(FrancescosRecipesWorldDbContext context)
        {
            _context = context;
        }

        public async Task<Category> GetCategoryByIdAsync(Guid categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            return category ?? throw new InvalidDataException($"Category {categoryId} not found.");
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<IEnumerable<Recipe>> GetRecipesByCategoryAsync(Guid categoryId)
        {
            var category = await _context.Categories
                .Include(c => c.Recipes)
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
            {
                throw new InvalidDataException($"Category {categoryId} not found.");
            }

            return category.Recipes;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesWithRecipesAsync()
        {
            var categories = await _context.Categories
       .Include(c => c.Recipes.OrderByDescending(r => r.CreatedAt).Take(3))
           .ThenInclude(r => r.MediaFiles.Take(2))
           .AsSplitQuery()
       .ToListAsync();

            return categories;
        }

        public async Task<IEnumerable<CategoryRecipesViewModel>> GetAllCategoriesWithRecipesViewModelAsync()
        {
            var categories = await _context.Categories
                .Select(c => new CategoryRecipesViewModel
                {
                    Category = c,
                    Recipes = c.Recipes
                        .OrderByDescending(r => r.CreatedAt)
                        .Take(3)
                        .Select(r => new RecipeCardViewModel
                        {
                            Id = r.Id,
                            Name = r.Name,
                            CookingTime = r.CookingTime,
                            IsFavorite = r.IsFavorite,
                            ImageData = r.MediaFiles
                                .OrderBy(m => m.Id)
                                .Select(m => m.Data)
                                .FirstOrDefault(),
                            MimeType = r.MediaFiles
                                .OrderBy(m => m.Id)
                                .Select(m => m.MimeType)
                                .FirstOrDefault(),
                        }),
                })
                .AsSplitQuery()
                .ToListAsync();

            return categories;
        }
    }
}
