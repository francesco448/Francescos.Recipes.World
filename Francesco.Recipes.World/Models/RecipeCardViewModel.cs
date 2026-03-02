namespace Francesco.Recipes.World.Models
{
    public class RecipeCardViewModel : IFavoritable
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public TimeSpan CookingTime { get; set; }

        public bool IsFavorite { get; set; }

        public byte[]? ImageData { get; set; }

        public string? MimeType { get; set; }
    }
}
