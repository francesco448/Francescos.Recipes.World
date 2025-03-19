namespace Francesco.Recipes.World.Models.BackendModels
{
    public interface ITimeStampedEntity
    {
        DateTime CreatedAt { get; set; }

        DateTime? ModifiedAt { get; set; }
    }
}
