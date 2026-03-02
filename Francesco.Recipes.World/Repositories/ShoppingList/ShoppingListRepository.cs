namespace Francesco.Recipes.World.Repositories.ShoppingList
{
    using Francesco.Recipes.World.Data;
    using Francesco.Recipes.World.Models.BackendModels.IngredientShoppingList;
    using Francesco.Recipes.World.Models.BackendModels.RecipeShoppingList;
    using Francesco.Recipes.World.Models.BackendModels.Shoppinglist;
    using Microsoft.EntityFrameworkCore;

    public class ShoppingListRepository : IShoppingListRepository
    {
        private readonly FrancescosRecipesWorldDbContext _context;

        public ShoppingListRepository(FrancescosRecipesWorldDbContext context)
        {
            _context = context;
        }

        public async Task<ShoppingList> AddIngredientsToShoppingListAsync(Guid recipeId, List<Guid> ingredientIds)
        {
            if (ingredientIds == null || !ingredientIds.Any())
            {
                throw new ArgumentNullException(nameof(ingredientIds));
            }

            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .FirstOrDefaultAsync(r => r.Id == recipeId);

            if (recipe == null)
            {
                throw new Exception("Recipe not found.");
            }

            var shoppingList = await _context.ShoppingLists
                .Include(sl => sl.RecipeShoppingList)
                    .ThenInclude(rsl => rsl.SelectedIngredients)
                .Include(sl => sl.RecipeShoppingList)
                    .ThenInclude(rsl => rsl.Recipe)
                .FirstOrDefaultAsync(sl => sl.RecipeShoppingList.Any(rsl => rsl.Recipe.Id == recipeId));

            var recipeIngredients = await _context.RecipeIngredients
                .Where(ri => ingredientIds.Contains(ri.Id) && ri.Recipe.Id == recipeId)
                .ToListAsync();

            if (!recipeIngredients.Any())
            {
                throw new Exception("No valid ingredients found.");
            }

            if (shoppingList == null)
            {
                shoppingList = new ShoppingList
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    RecipeShoppingList = new List<RecipeShoppingList>(),
                };

                var newRecipeList = new RecipeShoppingList
                {
                    Id = Guid.NewGuid(),
                    Recipe = recipe,
                    SelectedIngredients = recipeIngredients.Select(ri => new RecipeIngredientShoppingList
                    {
                        Id = Guid.NewGuid(),
                        RecipeIngredient = ri,
                        IsChecked = false,
                    }).ToList(),
                };

                shoppingList.RecipeShoppingList.Add(newRecipeList);
                _context.ShoppingLists.Add(shoppingList);
            }
            else
            {
                var existingRecipeList = shoppingList.RecipeShoppingList
                    .FirstOrDefault(rsl => rsl.Recipe.Id == recipeId);

                if (existingRecipeList == null)
                {
                    existingRecipeList = new RecipeShoppingList
                    {
                        Id = Guid.NewGuid(),
                        Recipe = recipe,
                        SelectedIngredients = new List<RecipeIngredientShoppingList>(),
                    };

                    shoppingList.RecipeShoppingList.Add(existingRecipeList);
                }

                foreach (var ri in recipeIngredients)
                {
                    if (!existingRecipeList.SelectedIngredients.Any(si => si.RecipeIngredient.Id == ri.Id))
                    {
                        existingRecipeList.SelectedIngredients.Add(new RecipeIngredientShoppingList
                        {
                            Id = Guid.NewGuid(),
                            RecipeIngredient = ri,
                            IsChecked = false,
                        });
                    }
                }

                shoppingList.ModifiedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return shoppingList;
        }

        public async Task<IList<ShoppingList>> GetAllShoppingListsAsync()
        {
            return await GetShoppingListQuery()
                .OrderByDescending(sl => sl.CreatedAt)
                .ToListAsync();
        }

        public async Task RemoveRecipeIfEmptyAsync(Guid shoppingListRecipeId)
        {
            var recipeEntry = await _context.RecipeShoppingLists
                .Include(r => r.SelectedIngredients)
                .Include(r => r.ShoppingList)
                    .ThenInclude(sl => sl.RecipeShoppingList)
                .FirstOrDefaultAsync(r => r.Id == shoppingListRecipeId);

            if (recipeEntry != null && !recipeEntry.SelectedIngredients.Any())
            {
                var shoppingList = recipeEntry.ShoppingList;

                _context.RecipeShoppingLists.Remove(recipeEntry);
                await _context.SaveChangesAsync();

                if (shoppingList != null && (shoppingList.RecipeShoppingList == null || !shoppingList.RecipeShoppingList.Any()))
                {
                    _context.ShoppingLists.Remove(shoppingList);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task RemoveRecipeFromShoppingListAsync(Guid recipeShoppingListId)
        {
            var recipeEntry = await _context.RecipeShoppingLists
       .Include(r => r.SelectedIngredients)
       .Include(r => r.ShoppingList)
           .ThenInclude(sl => sl.RecipeShoppingList)
       .FirstOrDefaultAsync(r => r.Id == recipeShoppingListId);

            if (recipeEntry != null)
            {
                var shoppingList = recipeEntry.ShoppingList;

                _context.RecipeIngredientsShoppingLists.RemoveRange(recipeEntry.SelectedIngredients);

                _context.RecipeShoppingLists.Remove(recipeEntry);
                await _context.SaveChangesAsync();

                if (shoppingList != null)
                {
                    await _context.Entry(shoppingList).Collection(sl => sl.RecipeShoppingList).LoadAsync();

                    if (!shoppingList.RecipeShoppingList.Any())
                    {
                        _context.ShoppingLists.Remove(shoppingList);
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task<int> CountAllRecipeShoppinglistsAsync()
        {
            return await _context.RecipeShoppingLists
                .CountAsync();
        }

        public async Task RemoveIngredientsFromShoppingListAsync(List<Guid> recipeIngredientShoppingListIds)
        {
            if (recipeIngredientShoppingListIds == null || !recipeIngredientShoppingListIds.Any())
            {
                throw new ArgumentNullException(nameof(recipeIngredientShoppingListIds));
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var affectedRecipeShoppingListIds = await _context.RecipeIngredientsShoppingLists
                    .Where(risl => recipeIngredientShoppingListIds.Contains(risl.Id))
                    .Select(risl => risl.RecipeShoppingList.Id)
                    .Distinct()
                    .ToListAsync();

                foreach (var id in recipeIngredientShoppingListIds)
                {
                    var entry = await _context.RecipeIngredientsShoppingLists.FindAsync(id);
                    if (entry != null)
                    {
                        _context.RecipeIngredientsShoppingLists.Remove(entry);
                    }
                }

                await _context.SaveChangesAsync();

                foreach (var recipeShoppingListId in affectedRecipeShoppingListIds)
                {
                    await RemoveRecipeIfEmptyAsync(recipeShoppingListId);
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private IQueryable<ShoppingList> GetShoppingListQuery()
        {
            return _context.ShoppingLists
                .Include(sl => sl.RecipeShoppingList)
                    .ThenInclude(rsl => rsl.Recipe)
                        .ThenInclude(r => r.MediaFiles)
                .Include(sl => sl.RecipeShoppingList)
                    .ThenInclude(rsl => rsl.SelectedIngredients)
                        .ThenInclude(si => si.RecipeIngredient)
                            .ThenInclude(ri => ri.Ingredient)
                .Include(sl => sl.RecipeShoppingList)
                    .ThenInclude(rsl => rsl.SelectedIngredients)
                        .ThenInclude(si => si.RecipeIngredient)
                            .ThenInclude(ri => ri.Unit);
        }
    }
}
