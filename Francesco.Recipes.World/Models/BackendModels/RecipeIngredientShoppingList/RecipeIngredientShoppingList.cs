namespace Francesco.Recipes.World.Models.BackendModels.IngredientShoppingList
{
    using Francesco.Recipes.World.Models.BackendModels.RecipeIngredient;
    using Francesco.Recipes.World.Models.BackendModels.RecipeShoppingList;

    public class RecipeIngredientShoppingList
    {
        public Guid Id { get; set; }

        public virtual RecipeShoppingList RecipeShoppingList { get; set; } = new ();

        public virtual RecipeIngredient RecipeIngredient { get; set; } = new ();

        public bool IsChecked { get; set; } = false;
    }
}
