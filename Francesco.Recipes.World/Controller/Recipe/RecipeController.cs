namespace Francesco.Recipes.World.Controller.Recipe
{
    using Francesco.Recipes.World.Models.BackendModels.Recipe;
    using Francesco.Recipes.World.Repositories.Category;
    using Francesco.Recipes.World.Repositories.Ingredient;
    using Francesco.Recipes.World.Repositories.Recipe;
    using Francesco.Recipes.World.Repositories.Unit;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    [Route("categories/{categoryId}/Recipe")]
    public class RecipeController : Controller
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IIngredientRepository _ingredientRepository;

        public RecipeController(IRecipeRepository recipeRepository, IUnitRepository unitRepository, ICategoryRepository categoryRepository, IIngredientRepository ingredientRepository)
        {
            _recipeRepository = recipeRepository;
            _unitRepository = unitRepository;
            _categoryRepository = categoryRepository;
            _ingredientRepository = ingredientRepository;
        }

        public IReadOnlyCollection<Recipe> Recipes { get; set; }

        // GET: /Recipe/AddOrCreateIngredient
        [HttpGet("{recipeId}/AddOrCreateIngredient")]
        public async Task<IActionResult> AddOrCreateIngredient()
        {
            var units = await _unitRepository.GetAllUnitsAsync();
            ViewBag.Units = new SelectList(units, "Id", "Name");
            return View();
        }

        // POST: /Recipe/AddOrCreateIngredient
        [HttpPost("{recipeId}/AddOrCreateIngredient")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrCreateIngredient(Guid recipeId, string ingredientName, int quantity, Guid unitId)
        {
            if (quantity <= 0)
            {
                ModelState.AddModelError(nameof(quantity), "Die Menge muss größer als 0 sein.");
            }

            if (!ModelState.IsValid)
            {
                var units = await _unitRepository.GetAllUnitsAsync();
                ViewBag.Units = new SelectList(units, "Id", "Name");
                return View();
            }

            await _recipeRepository.AddOrCreateIngredientToRecipeAsync(recipeId, ingredientName, quantity, unitId);
            return RedirectToAction("Details", new { id = recipeId });
        }

        // GET: /categories/{categoryId}/Recipe/Create
        [HttpGet("Create")]
        public async Task<IActionResult> Create(Guid categoryId)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (category == null)
            {
                return NotFound($"Kategorie mit ID {categoryId} wurde nicht gefunden.");
            }

            ViewBag.CategoryName = category.Name;
            return View();
        }

        // POST: /categories/{categoryId}/Recipe/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid categoryId, string name, string description, Difficulty difficulty, int servings, TimeSpan preparationTime, TimeSpan cookingTime)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError(nameof(name), "Name darf nicht leer sein.");
            }

            if (servings <= 0)
            {
                ModelState.AddModelError(nameof(servings), "Anzahl der Portionen muss größer als 0 sein.");
            }

            if (!ModelState.IsValid)
            {
                var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
                if (category == null)
                {
                    return NotFound($"Kategorie mit ID {categoryId} wurde nicht gefunden.");
                }

                ViewBag.CategoryName = category.Name;
                return View();
            }

            var categoryEntity = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (categoryEntity == null)
            {
                return NotFound($"Kategorie mit ID {categoryId} wurde nicht gefunden.");
            }

            await _recipeRepository.CreateRecipeForCategoryAsync(categoryEntity, name, description, difficulty, servings, preparationTime, cookingTime);
            return RedirectToAction("Details", "Category", new { id = categoryId });
        }

        // GET: /categories/{categoryId}/Recipe/{recipeId}/RemoveIngredient/{ingredientId}
        [HttpGet("{recipeId}/RemoveIngredient/{ingredientId}")]
        public async Task<IActionResult> RemoveIngredient(Guid categoryId, Guid recipeId, Guid ingredientId)
        {
            var recipe = await _recipeRepository.GetRecipeAsync(recipeId);
            var ingredient = await _ingredientRepository.GetIngredientByIdAsync(ingredientId);

            if (recipe == null || ingredient == null)
            {
                return NotFound("Recipe or Ingredient not found.");
            }

            ViewBag.RecipeId = recipeId;
            ViewBag.IngredientId = ingredientId;
            ViewBag.CategoryId = categoryId;
            ViewBag.IngredientName = ingredient.Name;

            return View();
        }

        // POST: /categories/{categoryId}/Recipe/{recipeId}/RemoveIngredient/{ingredientId}
        [HttpPost("{recipeId}/RemoveIngredient/{ingredientId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveIngredientConfirmed(Guid categoryId, Guid recipeId, Guid ingredientId)
        {
            await _recipeRepository.RemoveIngredientFromRecipeAsync(recipeId, ingredientId);
            TempData["SuccessMessage"] = "Ingredient removed successfully.";
            return RedirectToAction("Details", new { id = recipeId });
        }

        // GET: /categories/{categoryId}/Recipe/FilterByDifficulty
        [HttpGet("FilterByDifficulty")]
        public async Task<IActionResult> FilterByDifficulty(Difficulty? difficulty)
        {
            Recipes = await _recipeRepository.GetRecipesByDifficultyAsync(difficulty);
            return View(Recipes);
        }
    }
}
