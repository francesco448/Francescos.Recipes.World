namespace Francesco.Recipes.World.Models.BackendModels.Instruction
{
    using Francesco.Recipes.World.Models.BackendModels.MediaFile;
    using Francesco.Recipes.World.Models.BackendModels.Recipe;

    public class Instruction
    {
        public Guid Id { get; set; }

        public string Description { get; set; } = string.Empty;

        public int Number { get; set; }

        public virtual Recipe Recipe { get; set; } = new ();

        public virtual ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();
    }
}
