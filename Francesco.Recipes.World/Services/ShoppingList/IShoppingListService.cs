namespace Francesco.Recipes.World.Services.ShoppingList
{
    public interface IShoppingListService
    {
        Task<int> GetShoppingListRecipesCountAsync(Guid shoppingListId);
    }
}
