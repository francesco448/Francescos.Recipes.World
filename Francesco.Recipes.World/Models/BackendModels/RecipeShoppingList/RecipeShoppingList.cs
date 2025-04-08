namespace Francesco.Recipes.World.Models.BackendModels.RecipeShoppingList
{
    using Francesco.Recipes.World.Models.BackendModels.IngredientShoppingList;
    using Francesco.Recipes.World.Models.BackendModels.Recipe;
    using Francesco.Recipes.World.Models.BackendModels.Shoppinglist;

    public class RecipeShoppingList
    {
        public Guid Id { get; set; }

        public virtual ShoppingList ShoppingList { get; set; } = new ShoppingList();

        public virtual Recipe Recipe { get; set; } = new ();

        public virtual ICollection<RecipeIngredientShoppingList> SelectedIngredients { get; set; } = new List<RecipeIngredientShoppingList>();
    }
}
