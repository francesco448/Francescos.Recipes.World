namespace Francesco.Recipes.World.Models.BackendModels.Category
{
    using Francesco.Recipes.World.Models.BackendModels.Recipe;
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    }
}
