namespace Francesco.Recipes.World.Controllers
{
    using Francesco.Recipes.World.Data;
    using Francesco.Recipes.World.Models;
    using Francesco.Recipes.World.Repositories.ShoppingList;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class ShoppingListController : Controller
    {
        private readonly IShoppingListRepository _shoppingListRepository;
        private readonly FrancescosRecipesWorldDbContext _context;

        public ShoppingListController(IShoppingListRepository shoppingListRepository, FrancescosRecipesWorldDbContext context)
        {
            _shoppingListRepository = shoppingListRepository;
            _context = context;
        }

        [HttpPost("CreateOrAddIngredients")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrAddIngredients([FromBody] CreateOrAddIngredientRequestModel request)
        {
            if (request == null || request.IngredientIds == null || !request.IngredientIds.Any())
            {
                return BadRequest("No ingredients provided.");
            }

            await _shoppingListRepository.AddIngredientsToShoppingListAsync(request.RecipeId, request.IngredientIds);

            var shoppingList = await _context.ShoppingLists
                .Include(sl => sl.RecipeShoppingList)
                    .ThenInclude(rsl => rsl.Recipe)
                .FirstOrDefaultAsync(sl => sl.RecipeShoppingList.Any(rsl => rsl.Recipe.Id == request.RecipeId));

            if (shoppingList == null)
            {
                return BadRequest("Error creating shopping list.");
            }

            return Json(new { shoppingListId = shoppingList.Id });
        }
    }
}
