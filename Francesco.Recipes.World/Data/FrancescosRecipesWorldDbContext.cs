namespace Francesco.Recipes.World.Data
{
    using Francesco.Recipes.World.Models.BackendModels.Category;
    using Francesco.Recipes.World.Models.BackendModels.Ingredient;
    using Francesco.Recipes.World.Models.BackendModels.Instruction;
    using Francesco.Recipes.World.Models.BackendModels.Recipe;
    using Francesco.Recipes.World.Models.BackendModels.RecipeIngredient;
    using Francesco.Recipes.World.Models.BackendModels;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class FrancescosRecipesWorldDbContext : IdentityDbContext<IdentityUser>
    {
        public FrancescosRecipesWorldDbContext(DbContextOptions<FrancescosRecipesWorldDbContext> options) 
            : base(options) 
        { 
        }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Ingredient> Ingredients => Set<Ingredient>();
        public DbSet<Instruction> Instructions => Set<Instruction>();
        public DbSet<Recipe> Recipes => Set<Recipe>();
        public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is ITimeStampedEntity timeStampedEntity)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            timeStampedEntity.CreatedAt = DateTime.UtcNow;
                            break;
                        case EntityState.Modified:
                            timeStampedEntity.ModifiedAt = DateTime.UtcNow;
                            break;
                    }
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
