namespace Francesco.Recipes.World.Models.BackendModels.Shoppinglist
{
    using Francesco.Recipes.World.Models.BackendModels.RecipeShoppingList;

    public class ShoppingList : ITimeStampedEntity
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public virtual ICollection<RecipeShoppingList> RecipeShoppingList { get; set; } = new List<RecipeShoppingList>();
    }
}
