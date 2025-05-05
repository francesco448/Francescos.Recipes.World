namespace Francesco.Recipes.World.Repositories.Instruction
{
    using Francesco.Recipes.World.Models.BackendModels.Instruction;

    public interface IInstructionRepository
    {
        Task<Instruction> GetInstructionAsync(Guid instructionId);

        Task<Instruction> CreateInstructionWithImageToRecipeAsync(Guid recipeId, string description, IFormFile? photo);

        Task RemoveInstructionFromRecipeAsync(Guid recipeId, Guid instructionId);

        Task SwapInstructionNumbersAsync(Instruction a, Instruction b);

        Task<List<Instruction>> GetInstructionsOfRecipeAsync(Guid recipeId);

        Task RenumberInstructionsAsync(Guid recipeId);
    }
}
