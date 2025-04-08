namespace Francesco.Recipes.World.Views.Category
{
    using Francesco.Recipes.World.Models.BackendModels.Category;
    using Francesco.Recipes.World.Models.BackendModels.Recipe;

    public class CategoryRecipesViewModel
    {
        public Category Category { get; set; } = new ();

        public IEnumerable<Recipe> Recipes { get; set; } = new List<Recipe>();
    }
}
