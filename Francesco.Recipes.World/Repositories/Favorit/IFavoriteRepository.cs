namespace Francesco.Recipes.World.Repositories.Favorit
{
    using Francesco.Recipes.World.Models.BackendModels.Recipe;

    public interface IFavoriteRepository
    {
        Task<IEnumerable<Recipe>> GetFavoriteRecipesAsync();

        Task<bool> IsFavoriteAsync(Guid recipeId);

        Task AddFavoriteAsync(Guid recipeId);

        Task RemoveFavoriteAsync(Guid recipeId);
    }
}
