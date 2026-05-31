using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NzolaNet.API.Interfaces;

namespace NzolaNet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string folder = "images")
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Nenhum ficheiro enviado." });

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".mp4", ".webm" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return BadRequest(new { message = "Tipo de ficheiro nao permitido." });

            try
            {
                var path = await _fileService.UploadAsync(file, folder);
                var fullUrl = $"{Request.Scheme}://{Request.Host}{path}";
                return Ok(new { url = fullUrl, path });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
