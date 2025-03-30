namespace Francesco.Recipes.World.Repositories.MediaFile
{
    public interface IMediaFileRepository
    {
        Task ReplaceInstructionImageAsync(Guid instructionId, IFormFile? newPhoto);

        Task UploadInstructionImageAsync(Guid instructionId, IFormFile? photo);

        Task ReplaceRecipeImageAsync(Guid recipeId, IFormFile? newPhoto);

        Task UploadRecipeImageAsync(Guid recipeId, IFormFile? photo);
    }
}
