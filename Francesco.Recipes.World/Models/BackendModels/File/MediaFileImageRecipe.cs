namespace Francesco.Recipes.World.Models.BackendModels.File
{
    using Francesco.Recipes.World.Models.BackendModels.Recipe;

    public class MediaFileImageRecipe : MediaFile
    {
        public virtual Recipe Recipe { get; set; }
    }
}
