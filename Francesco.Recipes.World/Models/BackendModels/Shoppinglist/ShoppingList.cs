using Francesco.Recipes.World.Models.BackendModels.IngredientShoppingList;

namespace Francesco.Recipes.World.Models.BackendModels.Shoppinglist
{
    public class ShoppingList
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<IngredientsShoppingList> IngredientsShoppingLists { get; set; } = new List<IngredientsShoppingList>();
    }
}
