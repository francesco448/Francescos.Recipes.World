namespace Francesco.Recipes.World.Repositories.MediaFile
{
    public interface IMediaFileRepository
    {
        Task ReplaceInstructionImageAsync(Guid instructionId, Guid mediaFileIdToReplace, string fileName, string mimeType, byte[] newMediaData);

        Task UploadInstructionImageAsync(Guid instructionId, IFormFile? photo);

        Task UploadRecipeMediaAsync(Guid recipeId, IFormFile mediaFile);
    }
}
