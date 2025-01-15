namespace Francesco.Recipes.World.Models.BackendModels.Ingredient
{
    using Francesco.Recipes.World.Models.BackendModels.Unit;
    public class Ingredient
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual Unit Unit { get; set; } = new();
        public int Quantity { get; set; }
    }
}
