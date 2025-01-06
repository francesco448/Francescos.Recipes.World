namespace Francesco.Recipes.World.Models.BackendModels.Instruction
{
    using Francesco.Recipes.World.Models.BackendModels.Recipe;
    public class Instruction
    {
        public Guid Id { get; set; }
        public string? StepDescription { get; set; } 
        public string? StepNumber { get; set; }
        public virtual Recipe Recipe { get; set; } = new();
    }
}
