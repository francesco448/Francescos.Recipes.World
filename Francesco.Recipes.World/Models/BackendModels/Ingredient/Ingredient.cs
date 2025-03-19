namespace Francesco.Recipes.World.Models.BackendModels.Ingredient
{
    using Francesco.Recipes.World.Models.BackendModels.IngredientShoppingList;
    using Francesco.Recipes.World.Models.BackendModels.RecipeIngredient;

    public class Ingredient
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
        public virtual ICollection<IngredientsShoppingList> IngredientShoppingLists { get; set; } = new List<IngredientsShoppingList>();
    }
}
