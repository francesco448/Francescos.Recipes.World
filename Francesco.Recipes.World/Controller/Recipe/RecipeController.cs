namespace Francesco.Recipes.World.Controller.Recipe
{
    using Francesco.Recipes.World.Data;
    using Francesco.Recipes.World.Models;
    using Francesco.Recipes.World.Models.BackendModels.Ingredient;
    using Francesco.Recipes.World.Models.BackendModels.Instruction;
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
        private readonly FrancescosRecipesWorldDbContext _context;

        public IReadOnlyCollection<Recipe> Recipes { get; set; }

        public RecipeController(
            IRecipeRepository recipeRepository,
            IUnitRepository unitRepository,
            ICategoryRepository categoryRepository,
            IIngredientRepository ingredientRepository,
            IMediaFileRepository mediaFileRepository,
            IInstructionRepository instructionRepository,
            IFavoriteRepository favoriteRepository,
            FrancescosRecipesWorldDbContext context)
        {
            _recipeRepository = recipeRepository;
            _unitRepository = unitRepository;
            _categoryRepository = categoryRepository;
            _ingredientRepository = ingredientRepository;
            Recipes = new List<Recipe>();
            _mediaFileRepository = mediaFileRepository;
            _instructionRepository = instructionRepository;
            _favoriteRepository = favoriteRepository;
            _context = context;
        }

        // GET: /Recipe/{recipeId}/AddOrCreateIngredient
        [HttpGet("{recipeId}/AddOrCreateIngredient")]
        public async Task<IActionResult> AddOrCreateIngredient(Guid recipeId)
        {
            if (recipeId == Guid.Empty)
            {
                return BadRequest("Recipe ID cannot be empty.");
            }

            var units = await _unitRepository.GetAllUnitsAsync();
            var recipeIngredients = await _ingredientRepository.GetIngredientsByRecipeIdAsync(recipeId);
            var ingredients = recipeIngredients.Select(ri => ri.Ingredient).ToList();

            var viewModel = new IngredientViewModel
            {
                RecipeId = recipeId,
                Ingredients = ingredients,
                Units = units.ToList(),
            };

            return View(viewModel);
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
            if (recipeId == Guid.Empty)
            {
                return BadRequest("Recipe ID cannot be empty.");
            }

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

            await _recipeRepository.CreateRecipeIngredientAsync(recipeId, ingredientName, quantity, unitId);
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

            var units = await _unitRepository.GetAllUnitsAsync();

            var viewModel = new CreateRecipeViewModel
            {
                CategoryId = categoryId,
                CategoryName = category.Name,
                IngredientViewModel = new IngredientViewModel
                {
                    RecipeId = Guid.Empty,
                    Ingredients = new List<Ingredient>(),
                    Units = units.ToList(),
                },
                InstructionViewModel = new InstructionViewModel
                {
                    RecipeId = Guid.Empty,
                    Instructions = new List<Instruction>(),
                },
            };
            return View(viewModel);
        }

        // POST: /Recipe/Create/{categoryId}
        [HttpPost("Create/{categoryId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid categoryId, CreateRecipeViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "CreateRecipeViewModel cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Name darf nicht leer sein.");
            }

            if (model.Servings <= 0)
            {
                ModelState.AddModelError(nameof(model.Servings), "Anzahl der Portionen muss größer als 0 sein.");
            }

            if (!ModelState.IsValid)
            {
                var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
                if (category == null)
                {
                    return NotFound($"Kategorie mit ID {categoryId} wurde nicht gefunden.");
                }

                var units = await _unitRepository.GetAllUnitsAsync();

                if (model.IngredientViewModel == null)
                {
                    model.IngredientViewModel = new IngredientViewModel
                    {
                        RecipeId = Guid.Empty,
                        Ingredients = new List<Ingredient>(),
                        Units = units.ToList(),
                    };
                }
                else
                {
                    model.IngredientViewModel.Units = units.ToList();
                }

                model.CategoryName = category.Name;
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var categoryEntity = await _categoryRepository.GetCategoryByIdAsync(categoryId);
                if (categoryEntity == null)
                {
                    return NotFound($"Kategorie mit ID {categoryId} wurde nicht gefunden.");
                }

                model.PreparationTime = new TimeSpan(model.PrepHours, model.PrepMinutes, 0);
                model.CookingTime = new TimeSpan(model.CookHours, model.CookMinutes, 0);

                var recipe = await _recipeRepository.CreateRecipeForCategoryAsync(
                    categoryEntity,
                    model.Name,
                    model.Description ?? string.Empty,
                    model.Difficulty,
                    model.Servings,
                    model.PreparationTime,
                    model.CookingTime);

                if (model.Photo != null)
                {
                    await _mediaFileRepository.UploadRecipeMediaAsync(recipe.Id, model.Photo);
                }

                if (model.Video != null)
                {
                    await _mediaFileRepository.UploadRecipeMediaAsync(recipe.Id, model.Video);
                }

                if (model.IngredientViewModel?.Ingredients != null)
                {
                    foreach (var ingredient in model.IngredientViewModel.Ingredients)
                    {
                        var ri = ingredient.RecipeIngredients?.FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(ingredient.Name) && ri?.Quantity > 0 && ri?.Unit?.Id != null)
                        {
                            await _recipeRepository.CreateRecipeIngredientAsync(
                                recipe.Id,
                                ingredient.Name,
                                ri.Quantity,
                                ri.Unit.Id);
                        }
                    }
                }

                if (model.InstructionViewModel?.Instructions != null)
                {
                    for (var i = 0; i < model.InstructionViewModel.Instructions.Count; i++)
                    {
                        var instruction = model.InstructionViewModel.Instructions[i];
                        if (!string.IsNullOrWhiteSpace(instruction.Description))
                        {
                            var fileKey = $"InstructionViewModel.Instructions[{i}].MediaFile";
                            IFormFile? imageFile = null;

                            if (Request.Form.Files.Any(f => f.Name == fileKey))
                            {
                                imageFile = Request.Form.Files[fileKey];
                            }

                            await _instructionRepository.CreateInstructionAsync(
                                recipe.Id,
                                instruction.Description,
                                imageFile);
                        }
                    }
                }

                await transaction.CommitAsync();
                return RedirectToAction("Details", new { recipeId = recipe.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
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

        // GET: /Recipe/{recipeId}/RemoveInstruction/{instructionId}
        [HttpGet("{recipeId}/RemoveInstruction/{instructionId}")]
        public async Task<IActionResult> RemoveInstruction(Guid recipeId, Guid instructionId)
        {
            var recipe = await _recipeRepository.GetRecipeAsync(recipeId);

            if (recipe == null)
            {
                return NotFound("Recipe not found.");
            }

            var instruction = recipe.Instructions?.FirstOrDefault(i => i.Id == instructionId);
            if (instruction == null)
            {
                return NotFound("Instruction not found in the specified recipe.");
            }

            ViewBag.RecipeId = recipeId;
            ViewBag.InstructionId = instructionId;

            return View();
        }

        // POST: /Recipe/{recipeId}/RemoveInstruction/{instructionId}
        [HttpPost("{recipeId}/RemoveInstruction/{instructionId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveInstructionConfirmed(Guid recipeId, Guid instructionId)
        {
            try
            {
                await _instructionRepository.RemoveInstructionFromRecipeAsync(recipeId, instructionId);
                TempData["SuccessMessage"] = "Instruction removed successfully.";
                return RedirectToAction("Details", new { recipeId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while removing the instruction: {ex.Message}";
                return RedirectToAction("Details", new { recipeId });
            }
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

            var instructions = await _instructionRepository.GetInstructionsOfRecipeAsync(recipeId);

            var viewModel = new InstructionViewModel
            {
                RecipeId = recipeId,
                Instructions = instructions,
            };

            return View(viewModel);
        }

        // POST: /Recipe/{recipeId}/AddInstruction
        [HttpPost("{recipeId}/AddInstruction")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddInstruction(Guid recipeId, string description, IFormFile? image)
        {
            try
            {
                await _instructionRepository.CreateInstructionAsync(recipeId, description, image);

                var recipe = await _recipeRepository.GetRecipeByIdAsync(recipeId);
                var instructions = recipe?.Instructions?.ToList() ?? new List<Instruction>();

                var viewModel = new InstructionViewModel
                {
                    RecipeId = recipeId,
                    Instructions = instructions,
                };

                return RedirectToAction("AddInstruction", new { recipeId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
            var recipe = await _recipeRepository.GetRecipeByIdAsync(recipeId);
            if (recipe == null)
            {
                return NotFound("Recipe not found.");
            }

            return PartialView("_FavoriteButton", recipe);
        }

        // POST: /Recipe/RemoveFavorite
        [HttpPost("RemoveFavorite")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFavorite(Guid recipeId)
        {
            await _favoriteRepository.RemoveFavoriteAsync(recipeId);
            var recipe = await _recipeRepository.GetRecipeByIdAsync(recipeId);
            if (recipe == null)
            {
                return NotFound("Recipe not found.");
            }

            return PartialView("_FavoriteButton", recipe);
        }

        // GET: /Recipe/{recipeId}/GetIngredients
        [HttpGet("{recipeId}/GetIngredients")]
        public async Task<IActionResult> GetIngredients(Guid recipeId)
        {
            var recipeIngredients = (await _ingredientRepository.GetIngredientsByRecipeIdAsync(recipeId)).Select(ri => ri.Ingredient).ToList();
            var units = await _unitRepository.GetAllUnitsAsync();

            var viewModel = new IngredientViewModel
            {
                RecipeId = recipeId,
                Ingredients = recipeIngredients,
                Units = units,
            };

            return PartialView("_IngredientsPartial", viewModel);
        }

        // GET: /Recipe/{recipeId}/AdjustableIngredients
        [HttpGet("{recipeId}/AdjustableIngredients")]
        public async Task<IActionResult> GetAdjustableIngredients(Guid recipeId)
        {
            var recipe = await _recipeRepository.GetRecipeByIdAsync(recipeId);
            if (recipe == null)
            {
                return NotFound("Recipe not found.");
            }

            return PartialView("_AdjustableIngredientsPartial", recipe);
        }

        // GET: /Recipe/{recipeId}/Delete
        [HttpGet("{recipeId}/Delete")]
        public async Task<IActionResult> Delete(Guid recipeId)
        {
            var recipe = await _recipeRepository.GetRecipeByIdAsync(recipeId);
            if (recipe == null)
            {
                return NotFound("Recipe not found.");
            }

            return View(recipe);
        }

        // POST: /Recipe/{recipeId}/Delete
        [HttpPost("{recipeId}/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid recipeId)
        {
            try
            {
                await _recipeRepository.DeleteRecipeAsync(recipeId);
                TempData["SuccessMessage"] = "Rezept wurde erfolgreich gelöscht.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ein Fehler ist aufgetreten: {ex.Message}";
                return RedirectToAction("Details", new { recipeId });
            }
        }
    }
}
