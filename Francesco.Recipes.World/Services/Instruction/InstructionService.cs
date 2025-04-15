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

        public async Task MoveInstructionDownAsync(Guid instructionId)
        {
            var instruction = await _instructionRepository.GetInstructionWithRecipeAsync(instructionId);

            var instructions = await _instructionRepository.GetInstructionsByRecipeIdAsync(instruction.Recipe.Id);

            var maxStep = instructions.Max(i => i.Number);

            if (instruction.Number >= maxStep)
            {
                return;
            }

            var neighbor = instructions.FirstOrDefault(i => i.Number == instruction.Number + 1);

            if (neighbor != null)
            {
                await _instructionRepository.SwapInstructionOrderAsync(instruction, neighbor);
            }
        }

        public async Task MoveInstructionUpAsync(Guid instructionId)
        {
            var instruction = await _instructionRepository.GetInstructionWithRecipeAsync(instructionId);

            if (instruction.Number == 1)
            {
                return;
            }

            var instructions = await _instructionRepository.GetInstructionsByRecipeIdAsync(instruction.Recipe.Id);

            var neighbor = instructions.FirstOrDefault(i => i.Number == instruction.Number - 1);

            if (neighbor != null)
            {
                await _instructionRepository.SwapInstructionOrderAsync(instruction, neighbor);
            }
        }
    }
}
