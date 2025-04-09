namespace Francesco.Recipes.World.Repositories.Favorit
{
    using Francesco.Recipes.World.Data;
    using Francesco.Recipes.World.Models.BackendModels.Recipe;
    using Microsoft.EntityFrameworkCore;

    public class FavoritRepository : IFavoriteRepository
    {
        private readonly FrancescosRecipesWorldDbContext _context;

        public FavoritRepository(FrancescosRecipesWorldDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Recipe>> GetFavoriteRecipesAsync()
        {
            return await _context.Recipes
                          .Where(r => r.IsFavorite)
                          .ToListAsync();
        }

        public async Task<bool> IsFavoriteAsync(Guid recipeId)
        {
            return await _context.Recipes
                 .AnyAsync(r => r.Id == recipeId && r.IsFavorite);
        }

        public async Task AddFavoriteAsync(Guid recipeId)
        {
            var recipe = await _context.Recipes.FindAsync(recipeId);
            if (recipe != null && !recipe.IsFavorite)
            {
                recipe.IsFavorite = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveFavoriteAsync(Guid recipeId)
        {
            var recipe = await _context.Recipes.FindAsync(recipeId);
            if (recipe != null && recipe.IsFavorite)
            {
                recipe.IsFavorite = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}
