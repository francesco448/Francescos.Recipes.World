namespace Francesco.Recipes.World.Models.BackendModels.Recipe
{
    using Francesco.Recipes.World.Models.BackendModels.Category;
    using Francesco.Recipes.World.Models.BackendModels.Favorit;
    using Francesco.Recipes.World.Models.BackendModels.File;
    using Francesco.Recipes.World.Models.BackendModels.Instruction;
    using Francesco.Recipes.World.Models.BackendModels.RecipeIngredient;

    public class Recipe
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Difficulty Difficulty { get; set; }
        public int Servings { get; set; }
        public TimeSpan PreparationTime { get; set; }
        public TimeSpan CookingTime { get; set; }
        public bool IsFavorite { get; set; }
        public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
        public virtual ICollection<Instruction> Instructions { get; set; } = new List<Instruction>();
        public virtual ICollection<MediaFileImageRecipe> Images { get; set; } = new List<MediaFileImageRecipe>();
        public virtual ICollection<MediaFileVideoRecipe> Videos { get; set; } = new List<MediaFileVideoRecipe>();
        public virtual Category Category { get; set; } = new();
        public virtual Favorit Favorit { get; set; } = new();
    }
}
