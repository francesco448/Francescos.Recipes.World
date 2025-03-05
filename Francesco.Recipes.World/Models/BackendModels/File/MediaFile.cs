using Francesco.Recipes.World.Models.BackendModels.File;
using Francesco.Recipes.World.Models.BackendModels.Instruction;
using Francesco.Recipes.World.Models.BackendModels.Recipe;

public abstract class MediaFile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? FileName { get; set; }
    public string? MimeType { get; set; }
    public byte[]? Data { get; set; }
}
