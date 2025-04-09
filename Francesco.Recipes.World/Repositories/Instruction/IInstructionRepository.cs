namespace Francesco.Recipes.World.Repositories.Instruction
{
    using Francesco.Recipes.World.Models.BackendModels.Instruction;

    public interface IInstructionRepository
    {
        Task<Instruction> GetInstructionAsync(Guid instructionId);

        Task<Instruction> CreateInstructionToRecipeAsync(Guid recipeId, string description, int number);

        Task<List<Instruction>> GetInstructionsByRecipeIdAsync(Guid recipeId);

        Task RemoveInstructionFromRecipeAsync(Guid recipeId, Guid instructionId);
    }
}
