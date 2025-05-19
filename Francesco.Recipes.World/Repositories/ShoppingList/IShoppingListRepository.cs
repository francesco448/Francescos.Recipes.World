namespace Francesco.Recipes.World.Repositories.ShoppingList
{
    using Francesco.Recipes.World.Models.BackendModels.IngredientShoppingList;
    using Francesco.Recipes.World.Models.BackendModels.Recipe;
    using Francesco.Recipes.World.Models.BackendModels.Shoppinglist;

    public interface IShoppingListRepository
    {
        Task<ShoppingList> AddIngredientsToShoppingListAsync(Guid recipeId, List<Guid> ingredientIds);

        Task<IEnumerable<RecipeIngredientShoppingList>> GetIngredientsOfRecipeInListAsync(Guid shoppingListRecipeId);

        Task UpdateIngredientCheckedAsync(Guid shoppingListRecipeId, Guid recipeIngredientId, bool isChecked);

        Task RemoveRecipeIfEmptyAsync(Guid shoppingListRecipeId);

        Task DeleteShoppingListAsync(Guid shoppingListId);

        Task<Recipe?> GetRecipeByNameAndImageAsync(string recipeName, string imageFileName);

        Task<ShoppingList?> GetByIdAsync(Guid shoppingListId);

        Task<int> CountAllRecipeShoppinglistsAsync();
    }
}
