namespace Francesco.Recipes.World.Repositories.ShoppingList
{
    using Francesco.Recipes.World.Models.BackendModels.Shoppinglist;

    public interface IShoppingListRepository
    {
        Task<ShoppingList> AddIngredientsToShoppingListAsync(Guid recipeId, List<Guid> ingredientIds);

        Task RemoveRecipeIfEmptyAsync(Guid shoppingListRecipeId);

        Task<int> CountAllRecipeShoppinglistsAsync();

        Task<IList<ShoppingList>> GetAllShoppingListsAsync();

        Task RemoveIngredientsFromShoppingListAsync(List<Guid> recipeIngredientShoppngListIds);

        Task RemoveRecipeFromShoppingListAsync(Guid recipeShoppingListId);
    }
}
