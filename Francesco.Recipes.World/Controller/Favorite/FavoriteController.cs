namespace Francesco.Recipes.World.Controller.Favorite
{
    using Francesco.Recipes.World.Constants;
    using Francesco.Recipes.World.Models;
    using Francesco.Recipes.World.Repositories.Favorit;
    using Microsoft.AspNetCore.Mvc;

    public class FavoriteController : Controller
    {
        private readonly IFavoriteRepository _favoriteRepository;

        public FavoriteController(IFavoriteRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        public async Task<IActionResult> Index(string sortOrder = SortOrders.Newest)
        {
            var favoriteRecipes = await _favoriteRepository.GetFavoriteRecipesAsync();

            var sortedRecipes = sortOrder == SortOrders.Oldest
                ? favoriteRecipes.OrderBy(r => r.Favorite.CreatedAt)
                : favoriteRecipes.OrderByDescending(r => r.Favorite.CreatedAt);

            var viewModel = new FavoriteViewModel
            {
                FavoriteRecipes = sortedRecipes,
                SortOrder = sortOrder,
            };

            return View(viewModel);
        }
    }
}
