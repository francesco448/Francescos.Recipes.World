namespace Francesco.Recipes.World.Models.BackendModels.Unit
{
    using Francesco.Recipes.World.Models.BackendModels.Ingredient;

    public class Unit
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Ingredient> Recipes { get; set; } = new List<Ingredient>();
    }
}
