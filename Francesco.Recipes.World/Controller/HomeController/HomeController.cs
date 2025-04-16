namespace Francesco.Recipes.World.Controller.Home
{
    using Francesco.Recipes.World.Repositories.Category;
    using Francesco.Recipes.World.Repositories.Recipe;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IRecipeRepository _recipeRepository;

        public HomeController(ICategoryRepository categoryRepository, IRecipeRepository recipeRepository)
        {
            _categoryRepository = categoryRepository;
            _recipeRepository = recipeRepository;
        }

        [HttpGet("/Home/Search")]
        public async Task<IActionResult> Search(string query)
        {
            var recipes = await _recipeRepository.GetRecipesBySearchQueryAsync(query);
            return PartialView("_SearchResultsPartial", recipes);
        }
    }
}
