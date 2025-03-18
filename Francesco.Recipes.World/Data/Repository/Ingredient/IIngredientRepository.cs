namespace FrancescoRecipesWorld.Repositories
{
    using Francesco.Recipes.World.Models.BackendModels.Ingredient;
    using Francesco.Recipes.World.Models.BackendModels.Recipe;

    public interface IIngredientRepository
    {
        Task<Ingredient> CreateIngredientToRecipeAsync(int recipeId, string ingredientName);
        Task UpdateIngredientAsync(Ingredient ingredient);
        Task<List<Ingredient>> GetIngredientsByRecipeIdAsync(Guid recipeId);
        Task<List<Recipe>> GetRecipesByIngredientIdAsync(Guid ingredientId);
        Task<List<Ingredient>> GetIngredientsByNameAsync(string name);
        Task RemoveIngredientFromRecipeAsync(Recipe recipe, Guid ingredientId);
        Task<Ingredient> GetIngredientByIdAsync(Guid ingredientId);

    }
}
