namespace FrancescoRecipesWorld.Repositories
{
    using Francesco.Recipes.World.Data;
    using Francesco.Recipes.World.Models.BackendModels.Ingredient;
    using Francesco.Recipes.World.Models.BackendModels.Recipe;

    public class IngredientRepository : IIngredientRepository
    {
        private readonly FrancescosRecipesWorldDbContext _context;
        private readonly RecipeRepository _recipeRepository;
        public IngredientRepository(
            FrancescosRecipesWorldDbContext context, RecipeRepository recipeRepository)
        {
            _context = context;
            _recipeRepository = recipeRepository;
        }

        public async Task<Ingredient> CreateIngredientToRecipeAsync(int recipeId, string ingredientName)
        {
            var recipe = await _context.Recipes.FindAsync(recipeId);
            if (recipe == null)
            {
                throw new ArgumentException("Recipe not found", nameof(recipeId));
            }

            var newIngredient = new Ingredient
            {
                Name = ingredientName,
            };
            _context.Ingredients.Add(newIngredient);
            await _context.SaveChangesAsync();
            return newIngredient;
        }

        public Task RemoveIngredientFromRecipeAsync(Recipe recipe, Guid ingredientId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ingredient>> GetIngredientsByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ingredient>> GetIngredientsByRecipeIdAsync(Guid recipeId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Recipe>> GetRecipesByIngredientIdAsync(Guid ingredientId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateIngredientAsync(Ingredient ingredient)
        {
            throw new NotImplementedException();
        }

        public async Task<Ingredient> GetIngredientByIdAsync(Guid ingredientId)
        {
            var ingredient = await _context.Ingredients.FindAsync(ingredientId);
            return ingredient ?? throw new InvalidDataException($"Address {ingredientId} not found.");
        }
    }
}
