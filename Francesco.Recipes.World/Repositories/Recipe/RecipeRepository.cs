namespace Francesco.Recipes.World.Repositories.Recipe
{
    using Francesco.Recipes.World.Data;
    using Francesco.Recipes.World.Models;
    using Francesco.Recipes.World.Models.BackendModels.Category;
    using Francesco.Recipes.World.Models.BackendModels.Ingredient;
    using Francesco.Recipes.World.Models.BackendModels.Recipe;
    using Francesco.Recipes.World.Models.BackendModels.RecipeIngredient;
    using Francesco.Recipes.World.Repositories.Ingredient;
    using Francesco.Recipes.World.Repositories.Unit;
    using Microsoft.EntityFrameworkCore;

    public class RecipeRepository : IRecipeRepository
    {
        private readonly FrancescosRecipesWorldDbContext _context;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IUnitRepository _unitRepository;

        public RecipeRepository(
            FrancescosRecipesWorldDbContext context, IIngredientRepository ingredientRepository, IUnitRepository unitRepository)
        {
            _context = context;
            _ingredientRepository = ingredientRepository;
            _unitRepository = unitRepository;
        }

        public async Task<Recipe> GetRecipeAsync(Guid recipeId)
        {
            var recipe = await _context.Recipes.FindAsync(recipeId);
            return recipe ?? throw new InvalidDataException($"Address {recipeId} not found.");
        }

        public async Task<Recipe?> GetRecipeByIdAsync(Guid recipeId)
        {
            return await _context.Recipes
         .Include(r => r.RecipeIngredients)
             .ThenInclude(ri => ri.Ingredient)
         .Include(r => r.RecipeIngredients)
             .ThenInclude(ri => ri.Unit)
         .Include(r => r.MediaFiles)
         .Include(r => r.Instructions)
            .ThenInclude(i => i.MediaFiles)
         .FirstOrDefaultAsync(r => r.Id == recipeId);
        }

        public async Task CreateRecipeIngredientAsync(Guid recipeId, string ingredientName, int quantity, Guid unitId)
        {
            var recipe = await GetRecipeAsync(recipeId);
            var unit = await _unitRepository.GetUnitByIdAsync(unitId);

            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "Die Menge muss größer als 0 sein.");
            }

            var ingredients = await _ingredientRepository.GetIngredientsByNameAsync(ingredientName);
            var exactMatch = ingredients
                .FirstOrDefault(i => i.Name.Equals(ingredientName, StringComparison.OrdinalIgnoreCase));
            Ingredient ingredient;

            if (exactMatch == null)
            {
                ingredient = new Ingredient
                {
                    Id = Guid.NewGuid(),
                    Name = ingredientName,
                };
                _context.Ingredients.Add(ingredient);
                await _context.SaveChangesAsync();
            }
            else
            {
                ingredient = exactMatch;
            }

            var existingEntry = await _context.RecipeIngredients
                .FirstOrDefaultAsync(ri => ri.Recipe.Id == recipeId && ri.Ingredient.Id == ingredient.Id);
            if (existingEntry != null)
            {
                throw new InvalidOperationException("Das Rezept enthält diese Zutat bereits.");
            }

            var recipeIngredient = new RecipeIngredient
            {
                Recipe = recipe,
                Ingredient = ingredient,
                Unit = unit,
                Quantity = quantity,
            };

            _context.Add(recipeIngredient);
            await _context.SaveChangesAsync();
        }

        public async Task<Recipe> CreateRecipeForCategoryAsync(
       Category category,
       string name,
       string description,
       Difficulty difficulty,
       int servings,
       TimeSpan preparationTime,
       TimeSpan cookingTime)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category), "Category cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name), "Name cannot be empty.");
            }

            if (servings <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(servings), "Servings must be greater than 0.");
            }

            var recipe = new Recipe
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                Difficulty = difficulty,
                Servings = servings,
                PreparationTime = preparationTime,
                CookingTime = cookingTime,
                CreatedAt = DateTime.UtcNow,
                Category = category,
            };

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            return recipe;
        }

        public async Task RemoveIngredientFromRecipeAsync(Guid recipeId, Guid ingredientId)
        {
            var recipeIngredient = await _context.RecipeIngredients
                .FirstOrDefaultAsync(ri => ri.Recipe.Id == recipeId && ri.Ingredient.Id == ingredientId);

            if (recipeIngredient == null)
            {
                throw new ArgumentException("Diese Zutat ist nicht mit dem Rezept verknüpft.");
            }

            _context.RecipeIngredients.Remove(recipeIngredient);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<SearchViewModel>> SearchInRecipesAndIngredients(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await _context.Recipes
                        .OrderByDescending(r => r.CreatedAt)
                        .Take(20)
                        .Select(r => new SearchViewModel
                        {
                            Id = r.Id,
                            Name = r.Name,
                            Description = r.Description,
                            IsFavorite = r.IsFavorite,
                            ImageData = r.MediaFiles
                                .Where(m => m.MimeType != null && m.MimeType.StartsWith("image/"))
                                .Select(m => m.Data)
                                .FirstOrDefault(),
                            MimeType = r.MediaFiles
                                .Where(m => m.MimeType != null && m.MimeType.StartsWith("image/"))
                                .Select(m => m.MimeType)
                                .FirstOrDefault(),
                            Ingredients = r.RecipeIngredients.Select(ri => ri.Ingredient.Name).ToList(),
                            TotalTime = r.PreparationTime.Add(r.CookingTime),
                        })
                        .ToListAsync();
                }

                var normalizedSearchTerm = searchTerm.ToLower();

                return await _context.Recipes
                    .Where(r => EF.Functions.Like(r.Name.ToLower(), $"%{normalizedSearchTerm}%") ||
                                (r.Description != null && EF.Functions.Like(r.Description.ToLower(), $"%{normalizedSearchTerm}%")) ||
                                r.RecipeIngredients.Any(ri => EF.Functions.Like(ri.Ingredient.Name.ToLower(), $"%{normalizedSearchTerm}%")))
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(100)
                    .Select(r => new SearchViewModel
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description,
                        IsFavorite = r.IsFavorite,
                        ImageData = r.MediaFiles
                            .Where(m => m.MimeType != null && m.MimeType.StartsWith("image/"))
                            .Select(m => m.Data)
                            .FirstOrDefault(),
                        MimeType = r.MediaFiles
                            .Where(m => m.MimeType != null && m.MimeType.StartsWith("image/"))
                            .Select(m => m.MimeType)
                            .FirstOrDefault(),
                        Ingredients = r.RecipeIngredients.Select(ri => ri.Ingredient.Name).ToList(),
                        TotalTime = r.PreparationTime.Add(r.CookingTime),
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SearchInRecipesAndIngredientsOptimized: {ex.Message}");
                return new List<SearchViewModel>();
            }
        }

        public async Task<IReadOnlyCollection<Recipe>> GetRecipesByDifficultyAsync(Difficulty? difficulty)
        {
            if (!difficulty.HasValue)
            {
                return await _context.Recipes
                    .Include(r => r.Category)
                    .Include(r => r.RecipeIngredients)
                        .ThenInclude(ri => ri.Ingredient)
                    .ToListAsync();
            }

            return await _context.Recipes
                .Where(r => r.Difficulty == difficulty.Value)
                .Include(r => r.Category)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .ToListAsync();
        }

        public async Task<bool> DeleteRecipeAsync(Guid recipeId)
        {
            var recipe = await GetRecipeByIdAsync(recipeId);

            if (recipe == null)
            {
                return false;
            }

            if (recipe.RecipeIngredients != null && recipe.RecipeIngredients.Any())
            {
                _context.RecipeIngredients.RemoveRange(recipe.RecipeIngredients);
            }

            if (recipe.Instructions != null && recipe.Instructions.Any())
            {
                foreach (var instruction in recipe.Instructions)
                {
                    if (instruction.MediaFiles != null && instruction.MediaFiles.Any())
                    {
                        _context.MediaFiles.RemoveRange(instruction.MediaFiles);
                    }
                }

                _context.Instructions.RemoveRange(recipe.Instructions);
            }

            if (recipe.MediaFiles != null && recipe.MediaFiles.Any())
            {
                _context.MediaFiles.RemoveRange(recipe.MediaFiles);
            }

            if (recipe.Favorit != null && recipe.Favorit.Id != Guid.Empty)
            {
                _context.Remove(recipe.Favorit);
            }

            _context.Recipes.Remove(recipe);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
