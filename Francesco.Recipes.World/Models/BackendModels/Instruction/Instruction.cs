namespace Francesco.Recipes.World.Models.BackendModels.Instruction
{
    using Francesco.Recipes.World.Models.BackendModels.Recipe;
    public class Instruction
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Number { get; set; }
        public virtual Recipe Recipe { get; set; } = new();
    }
}
