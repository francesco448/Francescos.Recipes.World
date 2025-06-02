namespace Francesco.Recipes.World.Repositories.Category
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Francesco.Recipes.World.Data;
    using Francesco.Recipes.World.Models.BackendModels.Category;
    using Francesco.Recipes.World.Models.BackendModels.Recipe;
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
            return await _context.Categories
                .Include(c => c.Recipes)
                    .ThenInclude(r => r.MediaFiles)
                    .AsSplitQuery() // Use AsSplitQuery to optimize loading related data
                .ToListAsync();
        }
    }
}
