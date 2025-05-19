using Francesco.Recipes.World.Models.BackendModels.Shoppinglist;

namespace Francesco.Recipes.World.Models
{
    public class ShoppingListDetailsViewModel
    {
        public ShoppingList ShoppingList { get; set; } = new ShoppingList();

        public int RecipeCount { get; set; }
    }
}
