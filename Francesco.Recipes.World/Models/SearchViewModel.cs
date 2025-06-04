namespace Francesco.Recipes.World.Models
{
    public class SearchViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsFavorite { get; set; }

        public byte[]? ImageData { get; set; }

        public string? MimeType { get; set; }

        public List<string> Ingredients { get; set; } = new ();

        public TimeSpan TotalTime { get; set; }
    }
}
