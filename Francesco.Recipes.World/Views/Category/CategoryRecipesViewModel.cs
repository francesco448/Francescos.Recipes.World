namespace Francesco.Recipes.World.Views.Category
{
    using Francesco.Recipes.World.Models;
    using Francesco.Recipes.World.Models.BackendModels.Category;

    public class CategoryRecipesViewModel
    {
        public Category Category { get; set; } = new ();

        public IEnumerable<RecipeCardViewModel> Recipes { get; set; } = new List<RecipeCardViewModel>();
    }
}
