namespace Francesco.Recipes.World.Models.BackendModels.File
{
    using Francesco.Recipes.World.Models.BackendModels.Recipe;
    public class MediaFileVideoRecipe : MediaFile
    {
        public virtual Recipe Recipe { get; set; }
    }
}
