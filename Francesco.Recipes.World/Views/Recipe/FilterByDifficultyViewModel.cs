namespace Francesco.Recipes.World.Views.Recipe
{
    using System.Collections.Generic;
    using Francesco.Recipes.World.Models.BackendModels.Recipe;

    public class FilterByDifficultyViewModel
    {
        public Difficulty? SelectedDifficulty { get; set; }

        public IReadOnlyCollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    }
}
