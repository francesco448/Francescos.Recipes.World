namespace Francesco.Recipes.World.Repositories.MediaFile
{
    using Francesco.Recipes.World.Constants;
    using Francesco.Recipes.World.Data;
    using Francesco.Recipes.World.Models.BackendModels.MediaFile;
    using Francesco.Recipes.World.Models.BackendModels.Recipe;
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

        public async Task ReplaceInstructionImageAsync(Guid instructionId, Guid mediaFileIdToReplace, string fileName, string mimeType, byte[] newMediaData)
        {
            var instruction = await _instructionRepository.GetInstructionAsync(instructionId);

            var mediaToReplace = instruction.MediaFiles.FirstOrDefault(m => m.Id == mediaFileIdToReplace);
            if (mediaToReplace == null)
            {
                throw new InvalidOperationException("The specified media file does not exist.");
            }

            _context.MediaFiles.Remove(mediaToReplace);
            await _context.SaveChangesAsync();

            var newMedia = new MediaFile
            {
                Id = Guid.NewGuid(),
                FileName = fileName,
                MimeType = mimeType,
                Data = newMediaData,
                Instruction = instruction,
            };

            _context.MediaFiles.Add(newMedia);
            await _context.SaveChangesAsync();
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
                    Recipe = null,
                };

                _context.Add(instructionImage);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UploadRecipeMediaAsync(Guid recipeId, IFormFile mediaFile)
        {
            if (mediaFile == null || mediaFile.Length == 0)
            {
                return;
            }

            try
            {
                var recipe = await _recipeRepository.GetRecipeAsync(recipeId);

                var isImage = mediaFile.ContentType.StartsWith(ContentType.Image);
                var isVideo = mediaFile.ContentType.StartsWith("video/");

                if (!isImage && !isVideo)
                {
                    throw new InvalidOperationException("Only image or video files are allowed.");
                }

                if (isImage)
                {
                    await RemoveExistingMediaAsync(recipe, ContentType.Image);
                }
                else
                {
                    await RemoveExistingMediaAsync(recipe, "video/");
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
                    Instruction = null,
                };

                _context.MediaFiles.Add(newMedia);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while uploading the media file.", ex);
            }
        }

        private async Task RemoveExistingMediaAsync(Recipe recipe, string mediaTypePrefix)
        {
            var existingMedia = recipe.MediaFiles?.FirstOrDefault(m => m.MimeType?.StartsWith(mediaTypePrefix) == true);
            if (existingMedia != null)
            {
                _context.MediaFiles.Remove(existingMedia);
                await _context.SaveChangesAsync();
            }
        }
    }
}
