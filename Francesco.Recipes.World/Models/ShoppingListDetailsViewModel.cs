using Francesco.Recipes.World.Models.BackendModels.Recipe;
using Francesco.Recipes.World.Models.BackendModels.Shoppinglist;

namespace Francesco.Recipes.World.Models
{
    public class ShoppingListDetailsViewModel
    {
        public ShoppingList ShoppingList { get; set; } = new ShoppingList();

        public int RecipeCount { get; set; }

        public List<Recipe> RecipesInAnyShoppingList { get; set; } = new List<Recipe>();
    }
}
