namespace Francesco.Recipes.World.Models.BackendModels.Unit
{
    using Francesco.Recipes.World.Models.BackendModels.RecipeIngredient;

    public class Unit
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Symbol { get; set; } = string.Empty;

        public virtual ICollection<RecipeIngredient> RecipeIngredient { get; set; } = new List<RecipeIngredient>();
    }
}
