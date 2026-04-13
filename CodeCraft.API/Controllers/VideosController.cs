using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeCraft.API.Controllers;
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class VideosController : BaseController
{
    private readonly IVideoService _videoService;

    public VideosController(IVideoService videoService)
    {
        _videoService = videoService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var url = await _videoService.UploadVideoAsync(
            file.OpenReadStream(),
            file.FileName
        );

        return Ok(new { url });
    }
}
