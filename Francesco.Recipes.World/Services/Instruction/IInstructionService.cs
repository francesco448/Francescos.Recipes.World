namespace Francesco.Recipes.World.Services.Instruction
{
    using Francesco.Recipes.World.Models.BackendModels.Instruction;

    public interface IInstructionService
    {
        Task MoveInstructionUpAsync(Guid recipeId, Guid instructionId);

        Task MoveInstructionDownAsync(Guid recipeId, Guid instructionId);

        Task<List<Instruction>> GetSortedInstructionsAsync(Guid recipeId);
    }
}
