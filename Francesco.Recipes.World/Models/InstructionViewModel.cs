namespace Francesco.Recipes.World.Models
{
    using Francesco.Recipes.World.Models.BackendModels.Instruction;

    public class InstructionViewModel
    {
        public Guid RecipeId { get; set; }

        public List<Instruction> Instructions { get; set; } = new List<Instruction>();
    }
}
