namespace Francesco.Recipes.World.Repositories.MediaFile
{
    using Francesco.Recipes.World.Data;
    using Francesco.Recipes.World.Models.BackendModels.MediaFile;
    using Francesco.Recipes.World.Repositories.Instruction;
    using Francesco.Recipes.World.Repositories.Recipe;

    public class MediaFileRepository : IMediaFileRepository
    {
        private readonly IInstructionRepository _instructionRepository;
        private readonly FrancescosRecipesWorldDbContext _context;
        private readonly IRecipeRepository _recipeRepository;

        public MediaFileRepository(IInstructionRepository instructionRepository, FrancescosRecipesWorldDbContext context, IRecipeRepository recipeRepository)
        {
            _instructionRepository = instructionRepository;
            _context = context;
            _recipeRepository = recipeRepository;
        }

        public async Task ReplaceInstructionImageAsync(Guid instructionId, IFormFile? newPhoto)
        {
            if (newPhoto is null)
            {
                throw new ArgumentNullException(nameof(newPhoto));
            }

            var instruction = await _instructionRepository.GetInstructionAsync(instructionId);
            _context.RemoveRange(instruction.MediaFiles);
            await _context.SaveChangesAsync();

            await UploadInstructionImageAsync(instructionId, newPhoto);
        }

        public async Task ReplaceRecipeImageAsync(Guid recipeId, IFormFile? newPhoto)
        {
            if (newPhoto is null)
            {
                throw new ArgumentNullException(nameof(newPhoto));
            }

            var recipe = await _recipeRepository.GetRecipeAsync(recipeId);
            _context.RemoveRange(recipe.MediaFiles);
            await _context.SaveChangesAsync();

            await UploadInstructionImageAsync(recipeId, newPhoto);
        }

        public async Task UploadInstructionImageAsync(Guid instructionId, IFormFile? photo)
        {
            if (photo is null)
            {
                throw new ArgumentNullException(nameof(photo));
            }

            var instruction = await _instructionRepository.GetInstructionAsync(instructionId);

            using (var memoryStream = new MemoryStream())
            {
                await photo.CopyToAsync(memoryStream);

                var instructionImage = new MediaFile
                {
                    FileName = photo.FileName,
                    MimeType = photo.ContentType,
                    Data = memoryStream.ToArray(),
                    Instruction = instruction,
                };

                _context.Add(instructionImage);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UploadRecipeImageAsync(Guid recipeId, IFormFile? photo)
        {
            if (photo is null)
            {
                throw new ArgumentNullException(nameof(photo));
            }

            var recipe = await _recipeRepository.GetRecipeAsync(recipeId);

            using (var memoryStream = new MemoryStream())
            {
                await photo.CopyToAsync(memoryStream);

                var recipeImage = new MediaFile
                {
                    FileName = photo.FileName,
                    MimeType = photo.ContentType,
                    Data = memoryStream.ToArray(),
                    Recipe = recipe,
                };

                _context.Add(recipeImage);
                await _context.SaveChangesAsync();
            }
        }
    }
}
