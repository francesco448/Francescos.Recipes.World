namespace Francesco.Recipes.World.Controller.Recipe
{
    using Francesco.Recipes.World.Models.BackendModels.Recipe;
    using Francesco.Recipes.World.Repositories.Category;
    using Francesco.Recipes.World.Repositories.Favorit;
    using Francesco.Recipes.World.Repositories.Ingredient;
    using Francesco.Recipes.World.Repositories.Instruction;
    using Francesco.Recipes.World.Repositories.MediaFile;
    using Francesco.Recipes.World.Repositories.Recipe;
    using Francesco.Recipes.World.Repositories.Unit;
    using Francesco.Recipes.World.Views.Category;
    using Francesco.Recipes.World.Views.Recipe;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class RecipeController : Controller
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMediaFileRepository _mediaFileRepository;
        private readonly IInstructionRepository _instructionRepository;
        private readonly IFavoriteRepository _favoriteRepository;

        public IReadOnlyCollection<Recipe> Recipes { get; set; }

        public RecipeController(
            IRecipeRepository recipeRepository,
            IUnitRepository unitRepository,
            ICategoryRepository categoryRepository,
            IIngredientRepository ingredientRepository,
            IMediaFileRepository mediaFileRepository,
            IInstructionRepository instructionRepository,
            IFavoriteRepository favoriteRepository)
        {
            _recipeRepository = recipeRepository;
            _unitRepository = unitRepository;
            _categoryRepository = categoryRepository;
            _ingredientRepository = ingredientRepository;
            Recipes = new List<Recipe>();
            _mediaFileRepository = mediaFileRepository;
            _instructionRepository = instructionRepository;
            _favoriteRepository = favoriteRepository;
        }

        // GET: /Recipe/{recipeId}/AddOrCreateIngredient
        [HttpGet("{recipeId}/AddOrCreateIngredient")]
        public async Task<IActionResult> AddOrCreateIngredient(Guid recipeId)
        {
            var units = await _unitRepository.GetAllUnitsAsync();
            ViewBag.Units = new SelectList(units, "Id", "Name");
            ViewBag.RecipeId = recipeId;
            return View();
        }

        // GET: /Recipe/Details/{recipeId}
        [HttpGet("Details/{recipeId}")]
        public async Task<IActionResult> Details(Guid recipeId)
        {
            var recipe = await _recipeRepository.GetRecipeByIdAsync(recipeId);
            if (recipe == null)
            {
                return NotFound("Recipe not found.");
            }

            return View(recipe);
        }

        // GET: /Recipe/CategoryRecipes
        [HttpGet("CategoryRecipes")]
        public async Task<IActionResult> CategoryRecipes()
        {
            var categories = await _categoryRepository.GetAllCategoriesWithRecipesAsync();

            var viewModel = categories.Select(c => new CategoryRecipesViewModel
            {
                Category = c,
                Recipes = c.Recipes,
            }).ToList();

            return View(viewModel);
        }

        // POST: /Recipe/{recipeId}/AddOrCreateIngredient
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

        // GET: /Recipe/Create/{categoryId}
        [HttpGet("Create/{categoryId}")]
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

        // POST: /Recipe/Create/{categoryId}
        [HttpPost("Create/{categoryId}")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(Guid categoryId, string name, string description, Difficulty difficulty, int servings, TimeSpan preparationTime, TimeSpan cookingTime, IFormFile? photo)
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
            var recipe = await _recipeRepository.CreateRecipeForCategoryAsync(categoryEntity, name, description, difficulty, servings, preparationTime, cookingTime);
            if (photo != null)
            {
                await _mediaFileRepository.UploadRecipeMediaAsync(recipe.Id, photo);
            }

            return RedirectToAction("Details", "Category", new { id = categoryId });
        }

        // GET: /Recipe/{recipeId}/RemoveIngredient/{ingredientId}
        [HttpGet("{recipeId}/RemoveIngredient/{ingredientId}")]
        public async Task<IActionResult> RemoveIngredient(Guid recipeId, Guid ingredientId)
        {
            var recipe = await _recipeRepository.GetRecipeAsync(recipeId);
            var ingredient = await _ingredientRepository.GetIngredientByIdAsync(ingredientId);

            if (recipe == null || ingredient == null)
            {
                return NotFound("Recipe or Ingredient not found.");
            }

            ViewBag.RecipeId = recipeId;
            ViewBag.IngredientId = ingredientId;

            return View();
        }

        // POST: /Recipe/{recipeId}/RemoveIngredient/{ingredientId}
        [HttpPost("{recipeId}/RemoveIngredient/{ingredientId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveIngredientConfirmed(Guid recipeId, Guid ingredientId)
        {
            await _recipeRepository.RemoveIngredientFromRecipeAsync(recipeId, ingredientId);
            TempData["SuccessMessage"] = "Ingredient removed successfully.";
            return RedirectToAction("Details", new { id = recipeId });
        }

        // GET: /Recipe/FilterByDifficulty
        [HttpGet("FilterByDifficulty")]
        public async Task<IActionResult> FilterByDifficulty(Difficulty? selectedDifficulty)
        {
            var recipes = await _recipeRepository.GetRecipesByDifficultyAsync(selectedDifficulty);
            var viewModel = new FilterByDifficultyViewModel
            {
                SelectedDifficulty = selectedDifficulty,
                Recipes = recipes,
            };
            return View(viewModel);
        }

        // GET: /Recipe/{recipeId}/AddInstruction
        [HttpGet("{recipeId}/AddInstruction")]
        public async Task<IActionResult> AddInstruction(Guid recipeId)
        {
            var recipe = await _recipeRepository.GetRecipeByIdAsync(recipeId);
            if (recipe == null)
            {
                return NotFound("Recipe not found.");
            }

            ViewBag.RecipeId = recipeId;
            return View(recipe);
        }

        // POST: /Recipe/{recipeId}/AddInstruction
        [HttpPost("{recipeId}/AddInstruction")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddInstruction(Guid recipeId, string description, int number)
        {
            try
            {
                await _instructionRepository.CreateInstructionToRecipeAsync(recipeId, description, number);
                return RedirectToAction("AddInstruction", new { recipeId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var recipe = await _recipeRepository.GetRecipeByIdAsync(recipeId);
                ViewBag.RecipeId = recipeId;
                return View(recipe);
            }
        }

        // GET: /Recipe/Favorites
        [HttpGet("Favorites")]
        public async Task<IActionResult> Favorites()
        {
            var favoriteRecipes = await _favoriteRepository.GetFavoriteRecipesAsync();
            return View(favoriteRecipes);
        }

        // POST: /Recipe/AddFavorite
        [HttpPost("AddFavorite")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFavorite(Guid recipeId)
        {
            await _favoriteRepository.AddFavoriteAsync(recipeId);
            return RedirectToAction("Details", new { recipeId });
        }

        // POST: /Recipe/RemoveFavorite
        [HttpPost("RemoveFavorite")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFavorite(Guid recipeId)
        {
            await _favoriteRepository.RemoveFavoriteAsync(recipeId);
            return RedirectToAction("Details", new { recipeId });
        }
    }
}
