namespace Francesco.Recipes.World.Repositories.Unit
{
    using Francesco.Recipes.World.Models.BackendModels.Unit;

    public interface IUnitRepository
    {
        Task<Unit> GetUnitByIdAsync(Guid unitId);

        Task<Unit> AddUnitAsync(string name, string symbol);

        Task<IEnumerable<Unit>> GetAllUnitsAsync();
    }
}
