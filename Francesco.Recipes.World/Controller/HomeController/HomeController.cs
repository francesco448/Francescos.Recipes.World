namespace Francesco.Recipes.World.Controller.HomeController
{
    using Francesco.Recipes.World.Repositories.Category;
    using Francesco.Recipes.World.Repositories.Recipe;
    using Francesco.Recipes.World.Views.Category;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly ICategoryRepository _categoryRepository;

        public HomeController(ICategoryRepository categoryRepository, IRecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllCategoriesWithRecipesAsync();

            var viewModel = categories.Select(category => new CategoryRecipesViewModel
            {
                Category = category,
                Recipes = category.Recipes,
            });

            return View("Index", viewModel);
        }

        [HttpGet("/Home/Search")]
        public async Task<IActionResult> Search(string term)
        {
            var recipes = await _recipeRepository.SearchInRecipesAndIngredients(term);
            return PartialView("_SearchResultsPartial", recipes);
        }
    }
}
