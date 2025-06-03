using Francesco.Recipes.World.Constants;
using Francesco.Recipes.World.Models.BackendModels.Recipe;

namespace Francesco.Recipes.World.Models
{
    public class FavoriteViewModel
    {
        public IEnumerable<Recipe> FavoriteRecipes { get; set; } = new List<Recipe>();

        public string SortOrder { get; set; } = SortOrders.Newest;

        public bool HasFavorites => FavoriteRecipes.Any();

        public string SortOrderDisplayText => SortOrder == SortOrders.Oldest ? "Älteste Favorits" : "Neueste Favorits";
    }
}
