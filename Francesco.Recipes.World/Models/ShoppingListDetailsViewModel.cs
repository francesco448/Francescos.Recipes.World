using Francesco.Recipes.World.Models.BackendModels.Recipe;

namespace Francesco.Recipes.World.Models
{
    public class ShoppingListDetailsViewModel
    {
        public int RecipeCount { get; set; }

        public List<Recipe> RecipesInAnyShoppingList { get; set; } = new List<Recipe>();

        public Dictionary<Guid, Guid> RecipeIngredientToShoppingListMap { get; set; } = new Dictionary<Guid, Guid>();
    }
}
