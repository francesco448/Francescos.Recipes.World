namespace Francesco.Recipes.World.Models.BackendModels.IngredientShoppingList
{
    using Francesco.Recipes.World.Models.BackendModels.Ingredient;
    using Francesco.Recipes.World.Models.BackendModels.Shoppinglist;

    public class IngredientsShoppingList
    {
        public Guid Id { get; set; }
        public ShoppingList Shoppinglist { get; set; } = new();
        public Ingredient Ingredient { get; set; } = new();
    }
}
