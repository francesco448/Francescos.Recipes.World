

namespace Francesco.Recipes.World.Models.BackendModels.Unit
{
    using Francesco.Recipes.World.Models.BackendModels.Ingredient;
    using Francesco.Recipes.World.Models.BackendModels.RecipeIngredient;
    public class Unit
    {
        public Guid Id{ get; set; }
        public string? Name { get; set; }
        public string? Symbol { get; set; }
        public virtual ICollection<RecipeIngredient> RecipeIngredient { get; set; } = new List<RecipeIngredient>();
    } 
}
