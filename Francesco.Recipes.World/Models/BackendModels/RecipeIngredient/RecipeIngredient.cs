namespace Francesco.Recipes.World.Models.BackendModels.RecipeIngredient
{
    using Francesco.Recipes.World.Models.BackendModels.Ingredient;
    using Francesco.Recipes.World.Models.BackendModels.Recipe;
    public class RecipeIngredient
    {
        public Guid Id { get; set; }
        public Guid RecipeId { get; set; }
        public virtual Recipe Recipe { get; set; } = new();
        public virtual Ingredient Ingredient { get; set; } = new();
    }
}
