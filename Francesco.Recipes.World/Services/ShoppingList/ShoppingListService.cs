namespace Francesco.Recipes.World.Services.ShoppingList
{
    using Francesco.Recipes.World.Data;
    using Francesco.Recipes.World.Repositories.ShoppingList;

    public class ShoppingListService
    {
        private readonly FrancescosRecipesWorldDbContext _context;
        private readonly IShoppingListRepository _shoppingListRepository;

        public ShoppingListService(FrancescosRecipesWorldDbContext context, IShoppingListRepository shoppingListRepository)
        {
            _context = context;
            _shoppingListRepository = shoppingListRepository;
        }

        public async Task<int> GetShoppingListRecipeCountAsync(Guid shoppingListId)
        {
            return await _shoppingListRepository.GetShoppingListRecipeCountAsync(shoppingListId);
        }
    }
}
