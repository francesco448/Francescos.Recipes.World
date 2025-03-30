namespace Francesco.Recipes.World.Models.BackendModels.IngredientShoppingList
{
    using Francesco.Recipes.World.Models.BackendModels.RecipeIngredient;
    using Francesco.Recipes.World.Models.BackendModels.Shoppinglist;

    public class RecipeIngredientShoppingList
    {
        public Guid Id { get; set; }

        public virtual ShoppingList ShoppingList { get; set; } = new ();

        public virtual RecipeIngredient RecipeIngredient { get; set; } = new ();
    }
}
