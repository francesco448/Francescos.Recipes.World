namespace Francesco.Recipes.World.Models.BackendModels.Shoppinglist
{
    using Francesco.Recipes.World.Models.BackendModels.IngredientShoppingList;

    public class ShoppingList : ITimeStampedEntity
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public virtual ICollection<IngredientsShoppingList> IngredientsShoppingLists { get; set; } = new List<IngredientsShoppingList>();
    }
}
