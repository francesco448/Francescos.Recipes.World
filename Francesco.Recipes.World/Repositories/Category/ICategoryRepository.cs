namespace Francesco.Recipes.World.Repositories.Category
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Francesco.Recipes.World.Models.BackendModels.Category;
    using Francesco.Recipes.World.Models.BackendModels.Recipe;
    using Francesco.Recipes.World.Views.Category;

    public interface ICategoryRepository
    {
        Task<Category> GetCategoryByIdAsync(Guid categoryId);

        Task<IEnumerable<Category>> GetAllCategoriesAsync();

        Task<IEnumerable<Recipe>> GetRecipesByCategoryAsync(Guid categoryId);

        Task<IEnumerable<Category>> GetAllCategoriesWithRecipesAsync();

        Task<IEnumerable<CategoryRecipesViewModel>> GetAllCategoriesWithRecipesViewModelAsync();
    }
}
