namespace Francesco.Recipes.World.Services.Instruction
{
    public interface IInstructionService
    {
        Task MoveInstructionUpAsync(Guid recipeId, Guid instructionId);

        Task MoveInstructionDownAsync(Guid recipeId, Guid instructionId);
    }
}
