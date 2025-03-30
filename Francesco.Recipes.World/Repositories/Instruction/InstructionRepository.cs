namespace Francesco.Recipes.World.Repositories.Instruction
{
    using Francesco.Recipes.World.Data;
    using Francesco.Recipes.World.Models.BackendModels.Instruction;

    public class InstructionRepository : IInstructionRepository
    {
        private readonly FrancescosRecipesWorldDbContext _context;

        public InstructionRepository(FrancescosRecipesWorldDbContext context)
        {
            _context = context;
        }

        public async Task<Instruction> GetInstructionAsync(Guid instructionId)
        {
            var instruction = await _context.Instructions.FindAsync(instructionId);
            return instruction ?? throw new InvalidDataException($"Instruction {instructionId} not found.");
        }
    }
}
