namespace Francesco.Recipes.World.Repositories.Ingredient
{
    using Francesco.Recipes.World.Data;
    using Francesco.Recipes.World.Models.BackendModels.Ingredient;
    using Francesco.Recipes.World.Models.BackendModels.Recipe;
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

        public async Task<Ingredient> CreateIngredientToRecipeAsync(Recipe recipe, string ingredientName)
        {
            if (recipe is null)
            {
                throw new ArgumentException("Recipe not found", nameof(recipe));
            }

            var newIngredient = new Ingredient
            {
                Id = Guid.NewGuid(),
                Name = ingredientName,
            };

            var recipeIngredient = new RecipeIngredient
            {
                Id = Guid.NewGuid(),
                Recipe = recipe,
                Ingredient = newIngredient,
                Quantity = 1,
            };

            _context.Ingredients.Add(newIngredient);
            _context.RecipeIngredients.Add(recipeIngredient);
            await _context.SaveChangesAsync();

            return newIngredient;
        }

        public async Task RemoveIngredientFromRecipeAsync(Recipe recipe, Guid ingredientId)
        {
            if (recipe == null)
            {
                throw new ArgumentNullException(nameof(recipe));
            }

            var ingredientToRemove = recipe.RecipeIngredients.FirstOrDefault(i => i.Id == ingredientId);

            if (ingredientToRemove != null)
            {
                recipe.RecipeIngredients.Remove(ingredientToRemove);
                await _context.SaveChangesAsync();
            }
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

            _context.Ingredients.Update(existingIngredient);
            await _context.SaveChangesAsync();
        }

        public async Task<Ingredient> GetIngredientByIdAsync(Guid ingredientId)
        {
            var ingredient = await _context.Ingredients.FindAsync(ingredientId);
            return ingredient ?? throw new InvalidDataException($"Address {ingredientId} not found.");
        }
    }
}
