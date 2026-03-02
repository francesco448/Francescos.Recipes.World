
namespace Francesco.Recipes.World.Repositories.Ingredient
{
    using Francesco.Recipes.World.Data;
    using Francesco.Recipes.World.Models.BackendModels.Ingredient;
    using Francesco.Recipes.World.Models.BackendModels.RecipeIngredient;
    using Microsoft.EntityFrameworkCore;

    public class IngredientRepository : IIngredientRepository
    {
        private readonly FrancescosRecipesWorldDbContext _context;

        public IngredientRepository(
            FrancescosRecipesWorldDbContext context)
        {
            _context = context;
        }

        public async Task UpdateRecipeIngredientAsync(RecipeIngredient recipeIngredient)
        {
            if (recipeIngredient == null)
            {
                throw new ArgumentNullException(nameof(recipeIngredient));
            }

            var existingRecipeIngredient = await _context.RecipeIngredients
                .Include(ri => ri.Ingredient)
                .Include(ri => ri.Unit)
                .FirstOrDefaultAsync(ri => ri.Id == recipeIngredient.Id);

            if (existingRecipeIngredient == null)
            {
                throw new InvalidOperationException($"RecipeIngredient with ID {recipeIngredient.Id} not found.");
            }

            existingRecipeIngredient.Quantity = recipeIngredient.Quantity;
            existingRecipeIngredient.Ingredient = recipeIngredient.Ingredient;
            existingRecipeIngredient.Unit = recipeIngredient.Unit;

            await _context.SaveChangesAsync();
        }

        public async Task<List<Ingredient>> GetIngredientsByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return await _context.Ingredients.ToListAsync();
            }

            return await _context.Ingredients
                .Where(i => i.Name.ToLower().Contains(name.ToLower()))
                .ToListAsync();
        }

        public async Task<List<RecipeIngredient>> GetIngredientsByRecipeIdAsync(Guid recipeId)
        {
            return await _context.RecipeIngredients
                .Include(ri => ri.Ingredient)
                .Include(ri => ri.Unit)
                .Where(ri => ri.Recipe.Id == recipeId)
                .ToListAsync();
        }

        public async Task UpdateIngredientAsync(Ingredient ingredient)
        {
            if (ingredient == null)
            {
                throw new ArgumentNullException(nameof(ingredient));
            }

            var existingIngredient = await _context.Ingredients.FindAsync(ingredient.Id);
            if (existingIngredient == null)
            {
                throw new InvalidOperationException($"Ingredient with ID {ingredient.Id} not found.");
            }

            existingIngredient.Name = ingredient.Name;

            await _context.SaveChangesAsync();
        }

        public async Task<Ingredient> GetIngredientByIdAsync(Guid ingredientId)
        {
            var ingredient = await _context.Ingredients.FindAsync(ingredientId);
            return ingredient ?? throw new InvalidDataException($"Address {ingredientId} not found.");
        }
    }
}
