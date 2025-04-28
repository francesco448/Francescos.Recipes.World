namespace Francesco.Recipes.World.Services.Instruction
{
    using Francesco.Recipes.World.Models.BackendModels.Instruction;
    using Francesco.Recipes.World.Repositories.Instruction;

    public class InstructionService : IInstructionService
    {
        private readonly IInstructionRepository _instructionRepository;

        public InstructionService(IInstructionRepository instructionRepository)
        {
            _instructionRepository = instructionRepository;
        }

        public Task MoveInstructionUpAsync(Guid recipeId, Guid instructionId)
          => MoveInstructionAsync(recipeId, instructionId, moveUp: true);

        public Task MoveInstructionDownAsync(Guid recipeId, Guid instructionId)
            => MoveInstructionAsync(recipeId, instructionId, moveUp: false);

        public async Task<List<Instruction>> GetSortedInstructionsAsync(Guid recipeId)
        {
            var instructions = await _instructionRepository.GetInstructionsOfRecipeAsync(recipeId);
            return instructions.OrderBy(i => i.Number).ToList();
        }

        private async Task MoveInstructionAsync(Guid recipeId, Guid instructionId, bool moveUp)
        {
            var instructions = await _instructionRepository.GetInstructionsOfRecipeAsync(recipeId);

            var instruction = instructions.FirstOrDefault(i => i.Id == instructionId);

            if (instruction == null)
            {
                throw new InvalidDataException($"Instruction with ID {instructionId} not found.");
            }

            var minStep = 1;
            var maxStep = instructions.Max(i => i.Number);

            if ((moveUp && instruction.Number == minStep) || (!moveUp && instruction.Number >= maxStep))
            {
                return;
            }

            // Instructions are ordered by ascending numbers (1, 2, 3, ...).
            // Moving up means swapping with the instruction that has one number less (Number - 1).
            // Moving down means swapping with the instruction that has one number more (Number + 1).
            var targetNumber = moveUp ? instruction.Number - 1 : instruction.Number + 1;
            var neighbor = instructions.FirstOrDefault(i => i.Number == targetNumber);

            if (neighbor != null)
            {
                await _instructionRepository.SwapInstructionNumbersAsync(instruction, neighbor);
            }
        }
    }
}
