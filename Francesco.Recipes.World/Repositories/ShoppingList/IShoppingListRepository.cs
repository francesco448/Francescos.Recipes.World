namespace Francesco.Recipes.World.Repositories.ShoppingList
{
    using Francesco.Recipes.World.Models.BackendModels.IngredientShoppingList;
    using Francesco.Recipes.World.Models.BackendModels.Recipe;

    public interface IShoppingListRepository
    {
        Task AddIngredientsToShoppingListAsync(Guid shoppingListId, List<Guid> ingredientIds);

        Task<IEnumerable<RecipeIngredientShoppingList>> GetIngredientsOfRecipeInListAsync(Guid shoppingListRecipeId);

        Task UpdateIngredientCheckedAsync(Guid shoppingListRecipeId, Guid recipeIngredientId, bool isChecked);

        Task RemoveRecipeIfEmptyAsync(Guid shoppingListRecipeId);

        Task DeleteShoppingListAsync(Guid shoppingListId);

        Task<Recipe?> GetRecipeByNameAndImageAsync(string recipeName, string imageFileName);
    }
}
