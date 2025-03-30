namespace Francesco.Recipes.World.Repositories.Ingredient
{
    using Francesco.Recipes.World.Models.BackendModels.Ingredient;
    using Francesco.Recipes.World.Models.BackendModels.Recipe;
    using Francesco.Recipes.World.Models.BackendModels.RecipeIngredient;

    public interface IIngredientRepository
    {
        Task<Ingredient> CreateIngredientToRecipeAsync(Recipe recipe, string ingredientName);

        Task UpdateIngredientAsync(Ingredient ingredient);

        Task<List<RecipeIngredient>> GetIngredientsByRecipeIdAsync(Guid recipeId);

        Task<List<Ingredient>> GetIngredientsByNameAsync(string name);

        Task RemoveIngredientFromRecipeAsync(Recipe recipe, Guid ingredientId);

        Task<Ingredient> GetIngredientByIdAsync(Guid ingredientId);
    }
}
