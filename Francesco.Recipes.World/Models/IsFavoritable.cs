namespace Francesco.Recipes.World.Models
{
    public interface IFavoritable
    {
         Guid Id { get; set; }

         bool IsFavorite { get; set; }
    }
}
