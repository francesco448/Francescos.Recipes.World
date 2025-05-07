namespace Francesco.Recipes.World.Models.BackendModels.MediaFile
{
    using Francesco.Recipes.World.Models.BackendModels.Instruction;
    using Francesco.Recipes.World.Models.BackendModels.Recipe;

    public class MediaFile
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? FileName { get; set; }

        public string? MimeType { get; set; }

        public byte[]? Data { get; set; }

        public virtual Recipe? Recipe { get; set; } = null;

        public virtual Instruction? Instruction { get; set; } = null;
    }
}
