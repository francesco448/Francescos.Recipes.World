namespace Francesco.Recipes.World.Models
{
    public class FavoritButtonViewModel : IFavoritable
    {
        public Guid Id { get; set; }

        public bool IsFavorite { get; set; }
    }
}
