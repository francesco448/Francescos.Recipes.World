namespace Francesco.Recipes.World.Services.Instruction
{
    public interface IInstructionService
    {
        Task MoveInstructionUpAsync(Guid instructionId);

        Task MoveInstructionDownAsync(Guid instructionId);

    }
}
