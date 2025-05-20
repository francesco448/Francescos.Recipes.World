namespace Francesco.Recipes.World.Repositories.ShoppingList
{
    using Francesco.Recipes.World.Data;
    using Francesco.Recipes.World.Models.BackendModels.IngredientShoppingList;
    using Francesco.Recipes.World.Models.BackendModels.Recipe;
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

        public async Task<ShoppingList?> GetByIdAsync(Guid shoppingListId)
        {
            return await GetShoppingListQuery()
                .FirstOrDefaultAsync(sl => sl.Id == shoppingListId);
        }

        public async Task<IList<ShoppingList>> GetAllShoppingListsAsync()
        {
            return await GetShoppingListQuery()
                .OrderByDescending(sl => sl.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<RecipeIngredientShoppingList>> GetIngredientsOfRecipeInListAsync(Guid shoppingListRecipeId)
        {
            return await _context.RecipeIngredientsShoppingLists
                .Include(i => i.RecipeIngredient)
                    .ThenInclude(ri => ri.Ingredient)
                .Include(i => i.RecipeIngredient.Unit)
                .Where(i => i.RecipeShoppingList.Id == shoppingListRecipeId)
                .ToListAsync();
        }

        public async Task UpdateIngredientCheckedAsync(Guid shoppingListRecipeId, Guid recipeIngredientId, bool isChecked)
        {
            var item = await _context.RecipeIngredientsShoppingLists
                .FirstOrDefaultAsync(i =>
                    i.RecipeShoppingList.Id == shoppingListRecipeId &&
                    i.RecipeIngredient.Id == recipeIngredientId);

            if (item == null)
            {
                throw new Exception("Zutat nicht gefunden.");
            }

            item.IsChecked = isChecked;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveRecipeIfEmptyAsync(Guid shoppingListRecipeId)
        {
            var recipeEntry = await _context.RecipeShoppingLists
                .Include(r => r.SelectedIngredients)
                .FirstOrDefaultAsync(r => r.Id == shoppingListRecipeId);

            if (recipeEntry != null && !recipeEntry.SelectedIngredients.Any())
            {
                _context.RecipeShoppingLists.Remove(recipeEntry);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteShoppingListAsync(Guid shoppingListId)
        {
            var list = await _context.ShoppingLists
                .FirstOrDefaultAsync(sl => sl.Id == shoppingListId);

            if (list != null)
            {
                _context.ShoppingLists.Remove(list);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Recipe?> GetRecipeByNameAndImageAsync(string recipeName, string imageFileName)
        {
            return await _context.Recipes
                .Where(r => r.Name == recipeName && r.MediaFiles.Any(mf => mf.FileName == imageFileName))
                .Include(r => r.MediaFiles.Where(mf => mf.FileName == imageFileName))
                .FirstOrDefaultAsync();
        }

        public async Task<int> CountAllRecipeShoppinglistsAsync()
        {
            return await _context.RecipeShoppingLists
                .CountAsync();
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
