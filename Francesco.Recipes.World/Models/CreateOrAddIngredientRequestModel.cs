namespace Francesco.Recipes.World.Models
{
    public class CreateOrAddIngredientRequestModel
    {
        public Guid RecipeId { get; set; }

        public List<Guid> IngredientIds { get; set; } = new ();
    }
}
