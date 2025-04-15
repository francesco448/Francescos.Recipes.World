using Francesco.Recipes.World.Data;
using Microsoft.EntityFrameworkCore;

namespace Francesco.Recipes.World.Services.ShoppingList
{
    public class ShoppingListService
    {
        private readonly FrancescosRecipesWorldDbContext _context;

        public ShoppingListService(FrancescosRecipesWorldDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetShoppingListRecipeCountAsync(Guid shoppingListId)
        {
            return await _context.RecipeShoppingLists
                .Where(rsl => rsl.ShoppingList.Id == shoppingListId)
                .CountAsync();
        }
    }
}
