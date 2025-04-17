using Francesco.Recipes.World.Repositories.Instruction;

namespace Francesco.Recipes.World.Services.Instruction
{
    public class InstructionService : IInstructionService
    {
        private readonly IInstructionRepository _instructionRepository;

        public InstructionService(IInstructionRepository instructionRepository)
        {
            _instructionRepository = instructionRepository;
        }

        public Task MoveInstructionUpAsync(Guid instructionId)
          => MoveInstructionAsync(instructionId, moveUp: true);

        public Task MoveInstructionDownAsync(Guid instructionId)
            => MoveInstructionAsync(instructionId, moveUp: false);

        private async Task MoveInstructionAsync(Guid instructionId, bool moveUp)
        {
            var instructions = await _instructionRepository.GetInstructionsOfRecipeAsync(instructionId);

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

            var targetNumber = moveUp ? instruction.Number + 1 : instruction.Number - 1;
            var neighbor = instructions.FirstOrDefault(i => i.Number == targetNumber);

            if (neighbor != null)
            {
                await _instructionRepository.SwapInstructionNumbersAsync(instruction, neighbor);
            }
        }
    }
}
