namespace Francesco.Recipes.World.Repositories.Recipe
{
    using Francesco.Recipes.World.Data;
    using Francesco.Recipes.World.Models.BackendModels.Category;
    using Francesco.Recipes.World.Models.BackendModels.Ingredient;
    using Francesco.Recipes.World.Models.BackendModels.Recipe;
    using Francesco.Recipes.World.Models.BackendModels.RecipeIngredient;
    using Francesco.Recipes.World.Models.BackendModels.Unit;
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

        public async Task AddOrCreateIngredientToRecipeAsync(Guid recipeId, string ingredientName, int quantity, Guid unitId)
        {
            var recipe = await GetRecipeAsync(recipeId);
            var unit = await _unitRepository.GetUnitByIdAsync(unitId);

            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "Die Menge muss größer als 0 sein.");
            }

            var ingredients = await _ingredientRepository.GetIngredientsByNameAsync(ingredientName);
            Ingredient ingredient;

            if (ingredients == null || !ingredients.Any())
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
                ingredient = ingredients.First();
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
            var recipe = await GetRecipeAsync(recipeId);
            var recipeIngredient = await _context.RecipeIngredients
                .FirstOrDefaultAsync(ri => ri.Recipe.Id == recipeId && ri.Ingredient.Id == ingredientId);

            if (recipeIngredient == null)
            {
                throw new ArgumentException("Diese Zutat ist nicht mit dem Rezept verknüpft.");
            }

            _context.RecipeIngredients.Remove(recipeIngredient);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Recipe>> GetRecipesByNameOrIngredientAsync(string name, string ingredient)
        {
            var query = _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(r => r.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(ingredient))
            {
                var ingredientMatches = await _ingredientRepository.GetIngredientsByNameAsync(ingredient);
                var ingredientIds = ingredientMatches.Select(i => i.Id).ToList();

                if (ingredientIds.Any())
                {
                    query = query.Where(r => r.RecipeIngredients.Any(ri => ingredientIds.Contains(ri.Ingredient.Id)));
                }
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Recipe>> GetRecipesByDifficultyAsync(Difficulty difficulty)
        {
          return await _context.Recipes
              .Where(r => r.Difficulty == difficulty)
              .ToListAsync();
        }

        public async Task<Unit> AddUnitToRecipeAsync(string name, string symbol)
        {
            return await _unitRepository.AddUnitAsync(name, symbol);
        }
    }
}
