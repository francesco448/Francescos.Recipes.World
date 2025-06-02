namespace Francesco.Recipes.World.Controller.Favorite
{
    using Francesco.Recipes.World.Models;
    using Francesco.Recipes.World.Repositories.Favorit;
    using Microsoft.AspNetCore.Mvc;

    [Route("Favorite")]
    public class FavoriteController : Controller
    {
        private readonly IFavoriteRepository _favoriteRepository;

        public FavoriteController(IFavoriteRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        public async Task<IActionResult> Index(string sortOrder = "newest")
        {
            var favoriteRecipes = await _favoriteRepository.GetFavoriteRecipesAsync();

            var sortedRecipes = sortOrder == "oldest"
                ? favoriteRecipes.OrderBy(r => r.Favorit.CreatedAt)
                : favoriteRecipes.OrderByDescending(r => r.Favorit.CreatedAt);

            var viewModel = new FavoriteViewModel
            {
                FavoriteRecipes = sortedRecipes,
                SortOrder = sortOrder,
            };

            return View(viewModel);
        }
    }
}
