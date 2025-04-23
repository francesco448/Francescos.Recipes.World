namespace Francesco.Recipes.World.Controller.Instruction
{
    using Francesco.Recipes.World.Repositories.Instruction;
    using Francesco.Recipes.World.Services.Instruction;
    using Microsoft.AspNetCore.Mvc;

    public class InstructionController : Controller
    {
        private readonly IInstructionService _instructionService;
        private readonly IInstructionRepository _instructionRepository;

        public InstructionController(IInstructionService instructionService, IInstructionRepository instructionRepository)
        {
            _instructionService = instructionService;
            _instructionRepository = instructionRepository;
        }

        [HttpPost("Recipe/{recipeId}/Instruction/{instructionId}/move-up")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveUp(Guid recipeId, Guid instructionId)
        {
            try
            {
                await _instructionService.MoveInstructionUpAsync(recipeId, instructionId);
                return Ok(new { Message = "Instruction moved up successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("Recipe/{recipeId}/Instruction/{instructionId}/move-down")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveDown(Guid recipeId, Guid instructionId)
        {
            try
            {
                await _instructionService.MoveInstructionDownAsync(recipeId, instructionId);
                return Ok(new { Message = "Instruction moved down successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("Recipe/{recipeId}/Instructions")]
        public async Task<IActionResult> GetInstructions(Guid recipeId)
        {
            try
            {
                var instructions = await _instructionRepository.GetInstructionsOfRecipeAsync(recipeId);
                var sortedInstructions = instructions.OrderBy(i => i.Number).ToList();
                ViewData["RecipeId"] = recipeId;

                return View("~/Views/Shared/_GetInstructions.cshtml", sortedInstructions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
