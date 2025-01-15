namespace Francesco.Recipes.World.Models.BackendModels.Recipe
{
    using Francesco.Recipes.World.Models.BackendModels.Instruction;
    using Francesco.Recipes.World.Models.BackendModels.RecipeIngredient;

    public class Recipe
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Difficulty { get; set; }
        public int Servings { get; set; }
        public TimeSpan PreparationTime { get; set; }
        public TimeSpan CookingTime { get; set; }
        public virtual ICollection <RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
        public virtual ICollection <Instruction> Instructions { get; set; } = new List<Instruction>();
    }
}
