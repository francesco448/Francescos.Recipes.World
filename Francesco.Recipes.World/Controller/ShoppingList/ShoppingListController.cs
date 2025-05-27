namespace Francesco.Recipes.World.Controllers
{
    using Francesco.Recipes.World.Data;
    using Francesco.Recipes.World.Models;
    using Francesco.Recipes.World.Repositories.ShoppingList;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

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
        [HttpGet("Details")]
        public async Task<IActionResult> Details()
        {
            var recipeCount = await _shoppingListRepository.CountAllRecipeShoppinglistsAsync();

            var recipeIngredientToShoppingListMap = new Dictionary<Guid, Guid>();
            var allEntries = await _context.RecipeIngredientsShoppingLists
                .Where(risl => risl.RecipeIngredient != null)
                .Select(risl => new
                {
                    RecipeIngredientId = risl.RecipeIngredient.Id,
                    ShoppingListId = risl.Id,
                })
                .ToListAsync();

            foreach (var entry in allEntries)
            {
                if (entry.RecipeIngredientId != Guid.Empty &&
                    !recipeIngredientToShoppingListMap.ContainsKey(entry.RecipeIngredientId))
                {
                    recipeIngredientToShoppingListMap.Add(entry.RecipeIngredientId, entry.ShoppingListId);
                }
            }

            var recipeInAnyShoppingList = (await _shoppingListRepository.GetAllShoppingListsAsync())
                .SelectMany(sl => sl.RecipeShoppingList.Select(rsl => rsl.Recipe))
                .ToList();

            var viewModel = new ShoppingListDetailsViewModel
            {
                RecipeCount = recipeCount,
                RecipesInAnyShoppingList = recipeInAnyShoppingList,
                RecipeIngredientToShoppingListMap = recipeIngredientToShoppingListMap,
            };

            return View(viewModel);
        }

        [HttpDelete("RemoveIngredients")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveIngredients([FromBody] List<Guid> recipeIngredientShoppingListIds)
        {
            if (recipeIngredientShoppingListIds == null || !recipeIngredientShoppingListIds.Any())
            {
                return BadRequest("Keine Zutaten zum Entfernen angegeben.");
            }

            try
            {
                var affectedRecipeIds = await _context.RecipeIngredientsShoppingLists
                    .Where(risl => recipeIngredientShoppingListIds.Contains(risl.Id))
                    .Select(risl => risl.RecipeShoppingList.Recipe.Id)
                    .Distinct()
                    .ToListAsync();

                await _shoppingListRepository.RemoveIngredientsFromShoppingListAsync(recipeIngredientShoppingListIds);

                var remainingRecipeIds = await _context.RecipeShoppingLists
                    .Select(rsl => rsl.Recipe.Id)
                    .ToListAsync();

                var removedRecipeIds = affectedRecipeIds
                    .Where(id => !remainingRecipeIds.Contains(id))
                    .ToList();

                return Json(new { success = true, removedRecipeIds });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("RemoveRecipeFromList/{recipeId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRecipeFromList(Guid recipeId)
        {
            try
            {
                var recipeShoppingLists = await _context.RecipeShoppingLists
                    .Where(rsl => rsl.Recipe.Id == recipeId)
                    .ToListAsync();

                if (!recipeShoppingLists.Any())
                {
                    return NotFound($"Kein Einkaufslisten-Eintrag für Rezept mit ID {recipeId} gefunden.");
                }

                foreach (var recipeShoppingList in recipeShoppingLists)
                {
                    await _shoppingListRepository.RemoveRecipeFromShoppingListAsync(recipeShoppingList.Id);
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
    }
}
