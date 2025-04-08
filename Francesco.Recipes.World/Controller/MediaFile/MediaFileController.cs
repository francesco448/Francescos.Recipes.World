namespace Francesco.Recipes.World.Controller.MediaFile
{
    using Francesco.Recipes.World.Data;
    using Francesco.Recipes.World.Repositories.MediaFile;
    using Microsoft.AspNetCore.Mvc;

    [ValidateAntiForgeryToken]

    [Route("categories/{categoryId}/Recipe")]
    public class MediaFileController : Controller
    {
        private readonly IMediaFileRepository _mediaFileRepository;
        private readonly FrancescosRecipesWorldDbContext _context;

        public MediaFileController(IMediaFileRepository mediaFileRepository, FrancescosRecipesWorldDbContext context)
        {
            _mediaFileRepository = mediaFileRepository;
            _context = context;
        }

        // POST: /UploadImage
        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage(Guid recipeId, IFormFile? mediaFile)
        {
            if (mediaFile is null)
            {
                return BadRequest("Photo is required.");
            }

            try
            {
                await _mediaFileRepository.UploadRecipeMediaAsync(recipeId, mediaFile);
                return Ok("Image uploaded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: /UploadImage
        [HttpGet("UploadImage")]
        public IActionResult UploadImageView()
        {
            return View();
        }
    }
}
