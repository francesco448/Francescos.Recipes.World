namespace Francesco.Recipes.World.Controller.MediaFile
{
    using Francesco.Recipes.World.Repositories.MediaFile;
    using Microsoft.AspNetCore.Mvc;

    [Route("Category/{categoryId}/Recipe")]
    public class MediaFileController : Controller
    {
        private readonly IMediaFileRepository _mediaFileRepository;

        public MediaFileController(IMediaFileRepository mediaFileRepository)
        {
            _mediaFileRepository = mediaFileRepository;
        }

        // POST: /UploadImage
        [HttpPost("UploadImage")]
        [ValidateAntiForgeryToken]
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

        [HttpPost("ReplaceInstructionImage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReplaceInstructionImage(Guid instructionId, Guid mediaFileIdToReplace, IFormFile? newPhoto)
        {
            if (newPhoto is null)
            {
                return BadRequest("Photo is required.");
            }

            using (var memoryStream = new MemoryStream())
            {
                await newPhoto.CopyToAsync(memoryStream);

                var newMediaData = memoryStream.ToArray();

                try
                {
                    await _mediaFileRepository.ReplaceInstructionImageAsync(instructionId, mediaFileIdToReplace, newPhoto.FileName, newPhoto.ContentType, newMediaData);
                    return Ok("Image replaced successfully.");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }

        [HttpPost("Recipe/{recipeId}/Instruction/{instructionId}/UploadImage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadInstructionImage(Guid recipeId, Guid instructionId, IFormFile? photo)
        {
            if (photo == null)
            {
                return BadRequest("Photo is required.");
            }

            try
            {
                await _mediaFileRepository.UploadInstructionImageAsync(instructionId, photo);

                return RedirectToAction("GetInstructions", "Instruction", new { recipeId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
