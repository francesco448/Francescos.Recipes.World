using Francesco.Recipes.World.Models.BackendModels.Recipe;

namespace Francesco.Recipes.World.Models
{
    public class FavoriteViewModel
    {
        public IEnumerable<Recipe> FavoriteRecipes { get; set; } = new List<Recipe>();

        public string SortOrder { get; set; } = "newest";

        public bool HasFavorites => FavoriteRecipes.Any();

        public string SortOrderDisplayText => SortOrder == "oldest" ? "Älteste Favorits" : "Neueste Favorits";
    }
}
