namespace Francesco.Recipes.World.Repositories.Instruction
{
    using Francesco.Recipes.World.Models.BackendModels.Instruction;

    public interface IInstructionRepository
    {
        Task<Instruction> GetInstructionAsync(Guid instructionId);

        Task<Instruction> CreateInstructionToRecipeAsync(Guid recipeId, string description, int number);

        Task RemoveInstructionFromRecipeAsync(Guid recipeId, Guid instructionId);

        Task SwapInstructionOrderAsync(Instruction a, Instruction b);

        Task<List<Instruction>> GetInstructionsByInstructionIdAsync(Guid instructionId);
    }
}
