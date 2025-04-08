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

        public async Task ReplaceInstructionImageAsync(Guid instructionId, Guid mediaFileIdToReplace, IFormFile? newPhoto)
        {
            if (newPhoto is null)
            {
                throw new ArgumentNullException(nameof(newPhoto));
            }

            var instruction = await _instructionRepository.GetInstructionAsync(instructionId);

            var mediaToReplace = instruction.MediaFiles.FirstOrDefault(m => m.Id == mediaFileIdToReplace);
            if (mediaToReplace == null)
            {
                throw new InvalidOperationException("The specified media file does not exist.");
            }

            _context.MediaFiles.Remove(mediaToReplace);
            await _context.SaveChangesAsync();

            using (var memoryStream = new MemoryStream())
            {
                await newPhoto.CopyToAsync(memoryStream);

                var newMedia = new MediaFile
                {
                    Id = Guid.NewGuid(),
                    FileName = newPhoto.FileName,
                    MimeType = newPhoto.ContentType,
                    Data = memoryStream.ToArray(),
                    Instruction = instruction,
                };

                _context.MediaFiles.Add(newMedia);
                await _context.SaveChangesAsync();
            }
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

        public async Task UploadRecipeMediaAsync(Guid recipeId, IFormFile mediaFile)
        {
            if (mediaFile == null)
            {
                throw new ArgumentNullException(nameof(mediaFile));
            }

            var recipe = await _recipeRepository.GetRecipeAsync(recipeId);

            if (recipe == null)
            {
                throw new InvalidOperationException("The specified recipe does not exist.");
            }

            var isImage = mediaFile.ContentType.StartsWith("image/");
            var isVideo = mediaFile.ContentType.StartsWith("video/");

            if (!isImage && !isVideo)
            {
                throw new InvalidOperationException("Only image or video files are allowed.");
            }

            if (isImage)
            {
                var existingImage = recipe.MediaFiles?.FirstOrDefault(m => m.MimeType?.StartsWith("image/") == true);
                if (existingImage != null)
                {
                    _context.MediaFiles.Remove(existingImage);
                    await _context.SaveChangesAsync();
                }
            }
            else if (isVideo)
            {
                var existingVideo = recipe.MediaFiles?.FirstOrDefault(m => m.MimeType?.StartsWith("video/") == true);
                if (existingVideo != null)
                {
                    _context.MediaFiles.Remove(existingVideo);
                    await _context.SaveChangesAsync();
                }
            }

            using var memoryStream = new MemoryStream();
            await mediaFile.CopyToAsync(memoryStream);

            var newMedia = new MediaFile
            {
                Id = Guid.NewGuid(),
                FileName = mediaFile.FileName,
                MimeType = mediaFile.ContentType,
                Data = memoryStream.ToArray(),
                Recipe = recipe,
            };

            _context.MediaFiles.Add(newMedia);
            await _context.SaveChangesAsync();
        }
    }
}
