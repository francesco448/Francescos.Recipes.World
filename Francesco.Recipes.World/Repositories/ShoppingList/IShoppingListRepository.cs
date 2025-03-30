
namespace Francesco.Recipes.World.Repositories.ShoppingList
{
    using Francesco.Recipes.World.Models.BackendModels.Recipe;
    using Francesco.Recipes.World.Models.BackendModels.Shoppinglist;

    public interface IShoppingListRepository
    {
        Task AddIngredientToShoppingListAsync(Guid recipeIngredientId, Guid shoppingListId);

        Task RemoveRecipeIngredientFromShoppingListAsync(Guid recipeIngredientId);

        Task RemoveRecipeFromShoppingListIfEmptyAsync(Guid recipeId, Guid shoppingListId);

        Task<IEnumerable<ShoppingList>> GetShoppingListsByIngredientOfRecipeAsync(Guid ingredientId, Guid recipeId);

        Task<Recipe?> GetRecipeByNameAndImageAsync(string recipeName, string imageFileName);
    }
}
