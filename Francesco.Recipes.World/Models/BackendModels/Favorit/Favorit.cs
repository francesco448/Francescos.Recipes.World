namespace Francesco.Recipes.World.Models.BackendModels.Favorit
{
    using Francesco.Recipes.World.Models.BackendModels.Recipe;
    public class Favorit
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<Recipe> Recipe { get; set; } = new List<Recipe>();
    }
}
