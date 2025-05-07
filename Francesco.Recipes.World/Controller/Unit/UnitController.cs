namespace Francesco.Recipes.World.Controller.Unit
{
    using Francesco.Recipes.World.Repositories.Unit;
    using Microsoft.AspNetCore.Mvc;

    [Route("Unit")]
    public class UnitController : Controller
    {
        private readonly IUnitRepository _unitRepository;

        public UnitController(IUnitRepository unitRepository)
        {
            _unitRepository = unitRepository;
        }

        [HttpGet("GetAllUnits")]
        public async Task<IActionResult> GetAllUnits()
        {
            var units = (await _unitRepository.GetAllUnitsAsync())
                .Select(u => new { id = u.Id, name = u.Name })
                .ToList();
            return Json(units);
        }
    }
}
