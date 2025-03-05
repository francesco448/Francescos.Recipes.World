namespace Francesco.Recipes.World.Models.BackendModels.File
{
    using Francesco.Recipes.World.Models.BackendModels.Instruction;

    public class MediaFileImageInstruction : MediaFile
    {
        public virtual Instruction Instruction { get; set; }
    }
}
