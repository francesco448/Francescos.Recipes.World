namespace Francesco.Recipes.World.Controller.HomeController
{
    using Francesco.Recipes.World.Repositories.Category;
    using Francesco.Recipes.World.Repositories.Recipe;
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
            var categoriesWithRecipes = await _categoryRepository.GetAllCategoriesWithRecipesViewModelAsync();
            return View(categoriesWithRecipes);
        }

        [HttpGet("/Home/Search")]
        public async Task<IActionResult> Search(string term)
        {
            var recipes = await _recipeRepository.SearchInRecipesAndIngredients(term);
            return PartialView("_SearchResultsPartial", recipes);
        }
    }
}
