using Francesco.Recipes.World.Models.BackendModels.Instruction;

namespace Francesco.Recipes.World.Models
{
    public class InstructionViewModel
    {
        public Guid RecipeId { get; set; }

        public string Description { get; set; } = string.Empty;

        public List<Instruction> Instructions { get; set; } = new List<Instruction>();
    }
}
