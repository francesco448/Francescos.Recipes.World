namespace Francesco.Recipes.World.Repositories.Instruction
{
    using Francesco.Recipes.World.Data;
    using Francesco.Recipes.World.Models.BackendModels.Instruction;
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

        public async Task<Instruction> CreateInstructionToRecipeAsync(Guid recipeId, string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description cannot be empty", nameof(description));
            }

            var recipe = await _context.Recipes
                .Include(r => r.Instructions)
                .FirstOrDefaultAsync(r => r.Id == recipeId);

            if (recipe == null)
            {
                throw new ArgumentException("Recipe not found.", nameof(recipeId));
            }

            var nextNumber = recipe.Instructions?.Max(i => i.Number) ?? 0;
            nextNumber++;

            var newInstruction = new Instruction
            {
                Id = Guid.NewGuid(),
                Description = description,
                Number = nextNumber,
                Recipe = recipe,
            };

            _context.Instructions.Add(newInstruction);
            await _context.SaveChangesAsync();

            return newInstruction;
        }

        public async Task RemoveInstructionFromRecipeAsync(Guid recipeId, Guid instructionId)
        {
            var recipe = await _recipeRepository.GetRecipeAsync(recipeId);

            if (recipe.Instructions == null || !recipe.Instructions.Any())
            {
                await _context.Entry(recipe)
                    .Collection(r => r.Instructions)
                    .LoadAsync();
            }

            var instructionToRemove = recipe.Instructions?.FirstOrDefault(i => i.Id == instructionId);

            if (instructionToRemove != null)
            {
                await _context.Entry(instructionToRemove)
                    .Collection(i => i.MediaFiles)
                    .LoadAsync();

                if (instructionToRemove.MediaFiles != null && instructionToRemove.MediaFiles.Any())
                {
                    _context.MediaFiles.RemoveRange(instructionToRemove.MediaFiles);
                }

                recipe.Instructions?.Remove(instructionToRemove);
                await _context.SaveChangesAsync();
                await RenumberInstructionsAsync(recipeId);
            }
        }

        public async Task<List<Instruction>> GetInstructionsOfRecipeAsync(Guid recipeId)
        {
            var instructions = await _context.Instructions
                .Where(i => i.Recipe.Id == recipeId)
                .OrderBy(i => i.Number)
                .ToListAsync();

            if (!instructions.Any())
            {
                throw new InvalidDataException($"No instructions found for Recipe ID {recipeId}.");
            }

            return instructions;
        }

        public async Task RenumberInstructionsAsync(Guid recipeId)
        {
            var instructions = await _context.Instructions
                .Where(i => i.Recipe.Id == recipeId)
                .OrderBy(i => i.Number)
                .ToListAsync();

            for (var i = 0; i < instructions.Count; i++)
            {
                instructions[i].Number = i + 1;
            }

            await _context.SaveChangesAsync();
        }

        public async Task SwapInstructionNumbersAsync(Instruction a, Instruction b)
        {
            if (a == null)
            {
                throw new ArgumentNullException(nameof(a), "Instruction 'a' cannot be null.");
            }

            if (b == null)
            {
                throw new ArgumentNullException(nameof(b), "Instruction 'b' cannot be null.");
            }

            var temp = a.Number;
            a.Number = b.Number;
            b.Number = temp;

            await _context.SaveChangesAsync();
        }
    }
}
