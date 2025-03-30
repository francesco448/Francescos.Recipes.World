namespace Francesco.Recipes.World.Repositories.ShoppingList
{
    using Francesco.Recipes.World.Data;
    using Francesco.Recipes.World.Models.BackendModels.IngredientShoppingList;
    using Francesco.Recipes.World.Models.BackendModels.Recipe;
    using Francesco.Recipes.World.Models.BackendModels.Shoppinglist;
    using Microsoft.EntityFrameworkCore;

    public class ShoppingListRepository : IShoppingListRepository
    {
        private readonly FrancescosRecipesWorldDbContext _context;

        public ShoppingListRepository(FrancescosRecipesWorldDbContext context)
        {
            _context = context;
        }

        public async Task AddIngredientToShoppingListAsync(Guid recipeIngredientId, Guid shoppingListId)
        {
            var existingEntry = await _context.RecipeIngredientsShoppingLists
                .FirstOrDefaultAsync(risl => risl.RecipeIngredient.Id == recipeIngredientId && risl.ShoppingList.Id == shoppingListId);

            if (existingEntry != null)
            {
                return;
            }

            var recipeIngredient = await _context.RecipeIngredients
                .FirstOrDefaultAsync(ri => ri.Id == recipeIngredientId);

            if (recipeIngredient == null)
            {
                throw new InvalidOperationException($"Recipe ingredient with ID {recipeIngredientId} not found.");
            }

            var shoppingList = await _context.ShoppingLists
                .FirstOrDefaultAsync(sl => sl.Id == shoppingListId);

            if (shoppingList == null)
            {
                throw new InvalidOperationException($"Shopping list with ID {shoppingListId} not found.");
            }

            var newEntry = new RecipeIngredientShoppingList
            {
                RecipeIngredient = recipeIngredient,
                ShoppingList = shoppingList,
            };

            _context.RecipeIngredientsShoppingLists.Add(newEntry);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveRecipeIngredientFromShoppingListAsync(Guid recipeIngredientId)
        {
            var entry = await _context.RecipeIngredientsShoppingLists
                .FirstOrDefaultAsync(risl => risl.RecipeIngredient.Id == recipeIngredientId);

            if (entry != null)
            {
                _context.RecipeIngredientsShoppingLists.Remove(entry);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveRecipeFromShoppingListIfEmptyAsync(Guid recipeId, Guid shoppingListId)
        {
            var hasIngredients = await _context.RecipeIngredientsShoppingLists
                .AnyAsync(risl => risl.RecipeIngredient.Recipe.Id == recipeId && risl.ShoppingList.Id == shoppingListId);

            if (!hasIngredients)
            {
                var shoppingList = await _context.ShoppingLists
                    .Include(sl => sl.RecipeIngredientShoppingLists)
                    .FirstOrDefaultAsync(sl => sl.Id == shoppingListId);

                if (shoppingList != null)
                {
                    var recipeToRemove = shoppingList.RecipeIngredientShoppingLists
                        .FirstOrDefault(risl => risl.RecipeIngredient.Recipe.Id == recipeId);

                    if (recipeToRemove != null)
                    {
                        _context.RecipeIngredientsShoppingLists.Remove(recipeToRemove);
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task<IEnumerable<ShoppingList>> GetShoppingListsByIngredientOfRecipeAsync(Guid ingredientId, Guid recipeId)
        {
            return await _context.ShoppingLists
                .Where(sl => sl.RecipeIngredientShoppingLists.Any(risl => risl.RecipeIngredient.Ingredient.Id == ingredientId && risl.RecipeIngredient.Recipe.Id == recipeId))
                .ToListAsync();
        }

        public async Task<Recipe?> GetRecipeByNameAndImageAsync(string recipeName, string imageFileName)
        {
            return await _context.Recipes
                      .Where(r => r.Name == recipeName && r.MediaFiles.Any(mf => mf.FileName == imageFileName))
                      .Include(r => r.MediaFiles.Where(mf => mf.FileName == imageFileName))
                      .FirstOrDefaultAsync();
        }
    }
}
