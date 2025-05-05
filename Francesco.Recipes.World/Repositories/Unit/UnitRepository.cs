namespace Francesco.Recipes.World.Repositories.Unit
{
    using Francesco.Recipes.World.Data;
    using Francesco.Recipes.World.Models.BackendModels.Unit;
    using Microsoft.EntityFrameworkCore;

    public class UnitRepository : IUnitRepository
    {
        private readonly FrancescosRecipesWorldDbContext _context;

        public UnitRepository(
            FrancescosRecipesWorldDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> GetUnitByIdAsync(Guid unitId)
        {
            var unit = await _context.Units.FindAsync(unitId);
            return unit ?? throw new InvalidDataException($"Address {unitId} not found.");
        }

        public async Task<Unit> AddUnitAsync(string name, string symbol)
        {
            var unit = new Unit
            {
                Id = Guid.NewGuid(),
                Name = name,
                Symbol = symbol,
            };

            _context.Units.Add(unit);
            await _context.SaveChangesAsync();

            return unit;
        }

        public async Task<IList<Unit>> GetAllUnitsAsync()
        {
            return await _context.Units.ToListAsync();
        }
    }
}
