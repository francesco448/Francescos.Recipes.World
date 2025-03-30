namespace Francesco.Recipes.World.Models.BackendModels.Recipe;

using Francesco.Recipes.World.Models.BackendModels.Category;
using Francesco.Recipes.World.Models.BackendModels.Favorit;
using Francesco.Recipes.World.Models.BackendModels.Instruction;
using Francesco.Recipes.World.Models.BackendModels.MediaFile;
using Francesco.Recipes.World.Models.BackendModels.RecipeIngredient;

public class Recipe : ITimeStampedEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Difficulty Difficulty { get; set; }

    public int Servings { get; set; }

    public TimeSpan PreparationTime { get; set; }

    public TimeSpan CookingTime { get; set; }

    public bool IsFavorite { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();

    public virtual ICollection<Instruction> Instructions { get; set; } = new List<Instruction>();

    public virtual ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();

    public virtual Favorit Favorit { get; set; } = new ();

    public virtual Category Category { get; set; } = new ();
}
