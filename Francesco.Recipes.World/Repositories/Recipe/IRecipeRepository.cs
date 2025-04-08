namespace Francesco.Recipes.World.Repositories.Recipe
{
    using Francesco.Recipes.World.Models.BackendModels.Category;
    using Francesco.Recipes.World.Models.BackendModels.Recipe;

    public interface IRecipeRepository
    {
        Task<Recipe> GetRecipeAsync(Guid recipeId);

        Task<Recipe?> GetRecipeByIdAsync(Guid id);

        Task AddOrCreateIngredientToRecipeAsync(Guid recipeId, string ingredientName, int quantity, Guid unitId);

        Task RemoveIngredientFromRecipeAsync(Guid recipeId, Guid ingredientId);

        Task<IEnumerable<Recipe>> GetRecipesByNameOrIngredientAsync(string name, string ingredient);

        Task<IReadOnlyCollection<Recipe>> GetRecipesByDifficultyAsync(Difficulty? difficulty);

        Task<Recipe> CreateRecipeForCategoryAsync(Category category, string name, string description, Difficulty difficulty, int servings, TimeSpan preparationTime, TimeSpan cookingTime);
    }
}
