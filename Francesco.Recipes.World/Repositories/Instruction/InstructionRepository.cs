namespace Francesco.Recipes.World.Repositories.Instruction
{
    using Francesco.Recipes.World.Data;
    using Francesco.Recipes.World.Models.BackendModels.Instruction;
    using Francesco.Recipes.World.Models.BackendModels.Recipe;
    using Francesco.Recipes.World.Repositories.Recipe;
    using Microsoft.EntityFrameworkCore;

    public class InstructionRepository : IInstructionRepository
    {
        private readonly FrancescosRecipesWorldDbContext _context;
        private readonly IRecipeRepository _recipeRepository;

        public InstructionRepository(FrancescosRecipesWorldDbContext context, IRecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository;
            _context = context;
        }

        public async Task<Instruction> GetInstructionAsync(Guid instructionId)
        {
            var instruction = await _context.Instructions.FindAsync(instructionId);
            return instruction ?? throw new InvalidDataException($"Instruction {instructionId} not found.");
        }

        public async Task<Instruction> CreateInstructionToRecipeAsync(Guid recipeId, string description, int number)
        {
         var recipe = await _recipeRepository.GetRecipeAsync(recipeId);

         if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description cannot be empty", nameof(description));
            }

         if (number <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(number), "Number must be greater than 0.");
            }

         var newInstruction = new Instruction
            {
                Id = Guid.NewGuid(),
                Description = description,
                Number = number,
                Recipe = recipe,
            };

         _context.Instructions.Add(newInstruction);
         await _context.SaveChangesAsync();

         return newInstruction;
        }

        public async Task RemoveInstructionFromRecipeAsync(Recipe recipe, Guid instructionId)
        {
            if (recipe == null)
            {
                throw new ArgumentNullException(nameof(recipe));
            }

            var instructionToRemove = recipe.Instructions.FirstOrDefault(i => i.Id == instructionId);

            if (instructionToRemove != null)
            {
                recipe.Instructions.Remove(instructionToRemove);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Instruction>> GetInstructionsByRecipeIdAsync(Guid recipeId)
        {
            return await _context.Instructions
                .Include(i => i.Recipe)
                .Where(i => i.Recipe.Id == recipeId)
                .ToListAsync();
        }
    }
}
