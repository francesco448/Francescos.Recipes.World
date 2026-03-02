using Francesco.Recipes.World.Models.BackendModels.Recipe;

namespace Francesco.Recipes.World.Models
{
    public class CreateRecipeViewModel
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public Difficulty Difficulty { get; set; }

        public int Servings { get; set; }

        public TimeSpan PreparationTime { get; set; }

        public TimeSpan CookingTime { get; set; }

        public int PrepHours { get; set; }

        public int PrepMinutes { get; set; }

        public int CookHours { get; set; }

        public int CookMinutes { get; set; }

        public Guid CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public IFormFile? Photo { get; set; }

        public IFormFile? Video { get; set; }

        public IngredientViewModel? IngredientViewModel { get; set; }

        public InstructionViewModel? InstructionViewModel { get; set; }
    }
}
