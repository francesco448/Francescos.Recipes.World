namespace Francesco.Recipes.World.Controller.Category
{
    using Francesco.Recipes.World.Repositories.Category;
    using Microsoft.AspNetCore.Mvc;

    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // GET: /Category
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return View(categories);
        }

        // GET: /Category/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            return View(category);
        }

        // GET: /Category/{id}/recipes
        [HttpGet("{id:guid}/recipes")]
        public async Task<IActionResult> GetRecipesByCategory(Guid id)
        {
            var recipes = await _categoryRepository.GetRecipesByCategoryAsync(id);
            return Ok(recipes);
        }
    }
}
