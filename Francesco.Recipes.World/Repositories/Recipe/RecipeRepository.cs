namespace Francesco.Recipes.World.Repositories.Recipe
{
    using Francesco.Recipes.World.Data;
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
         .FirstOrDefaultAsync(r => r.Id == recipeId);
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

        public async Task<IEnumerable<Recipe>> SerachRecipeAndIngredientAsync(string searchTerm)
        {
            var queryable = _context.Recipes
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                queryable = queryable.Where(r => r.Name.ToLower().Contains(searchTerm) ||
                                                 r.RecipeIngredients.Any(ri => ri.Ingredient.Name.ToLower().Contains(searchTerm)));
            }

            return await queryable.ToListAsync();
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
    }
}
