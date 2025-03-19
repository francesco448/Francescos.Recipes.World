namespace Francesco.Recipes.World.Models.BackendModels.RecipeIngredient
{
    using Francesco.Recipes.World.Models.BackendModels.Ingredient;
    using Francesco.Recipes.World.Models.BackendModels.Recipe;
    using Francesco.Recipes.World.Models.BackendModels.Unit;
    public class RecipeIngredient
    {
        public Guid Id { get; set; }
        public virtual Recipe Recipe { get; set; } = new();
        public virtual Ingredient Ingredient { get; set; } = new();
        public virtual Unit Unit { get; set; } = new();
        public int Quantity { get; set; }
    }
}
