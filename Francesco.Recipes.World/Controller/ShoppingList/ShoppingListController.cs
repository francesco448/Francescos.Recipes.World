namespace Francesco.Recipes.World.Controllers
{
    using Francesco.Recipes.World.Data;
    using Francesco.Recipes.World.Models;
    using Francesco.Recipes.World.Repositories.ShoppingList;
    using Microsoft.AspNetCore.Mvc;

    [Route("ShoppingList")]
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request == null || request.IngredientIds == null || !request.IngredientIds.Any())
            {
                return BadRequest("No ingredients provided.");
            }

            var shoppingList = await _shoppingListRepository.AddIngredientsToShoppingListAsync(request.RecipeId, request.IngredientIds);

            if (shoppingList == null)
            {
                return BadRequest("Error creating shopping list.");
            }

            return Json(new { shoppingListId = shoppingList.Id });
        }

        [HttpGet("RecipeCount")]
        public async Task<IActionResult> RecipeCount()
        {
            var count = await _shoppingListRepository.CountAllRecipeShoppinglistsAsync();
            return Json(new { count });
        }

        // GET: /ShoppingList/Details/{id}
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var shoppingList = await _shoppingListRepository.GetByIdAsync(id);
            var recipeCount = await _shoppingListRepository.CountAllRecipeShoppinglistsAsync();

            if (shoppingList == null)
            {
                return NotFound();
            }

            var recipeInAnyShoppingList = (await _shoppingListRepository.GetAllShoppingListsAsync())
                .SelectMany(sl => sl.RecipeShoppingList.Select(rsl => rsl.Recipe))
                .ToList();

            var viewModel = new ShoppingListDetailsViewModel
            {
                ShoppingList = shoppingList,
                RecipeCount = recipeCount,
                RecipesInAnyShoppingList = recipeInAnyShoppingList,
            };

            return View(viewModel);
        }
    }
}
