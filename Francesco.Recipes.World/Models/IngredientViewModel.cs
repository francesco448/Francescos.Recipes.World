namespace Francesco.Recipes.World.Models
{
    using Francesco.Recipes.World.Models.BackendModels.Ingredient;
    using Francesco.Recipes.World.Models.BackendModels.Unit;

    public class IngredientViewModel
    {
        public Guid RecipeId { get; set; }

        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

        public List<Unit> Units { get; set; } = new List<Unit>();
    }
}
