namespace Francesco.Recipes.World.Repositories.MediaFile
{
    public interface IMediaFileRepository
    {
        Task ReplaceInstructionImageAsync(Guid instructionId, Guid mediafileId, IFormFile? newPhoto);

        Task UploadInstructionImageAsync(Guid instructionId, IFormFile? photo);

        Task UploadRecipeMediaAsync(Guid recipeId, IFormFile mediaFile);
    }
}
