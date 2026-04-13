using CodeCraft.Application.DTOs.Attachment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeCraft.API.Controllers;
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class AttachmentsController : BaseController
{
    private readonly ILessonAttachmentService _service;

    public AttachmentsController(ILessonAttachmentService service)
    {
        _service = service;
    }

    [HttpGet("lesson/{lessonId}")]
    public async Task<IActionResult> GetByLesson(int lessonId)
    {
        var result = await _service.GetByLessonIdAsync(lessonId);
        return Ok(result);
    }

   
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file, int lessonId)
    {
        var dto = new CreateAttachmentDto
        {
            FileName = file.FileName,
            FileStream = file.OpenReadStream(),
            LessonId = lessonId
        };

        var result = await _service.UploadAsync(dto);

        return Ok(result);
    }

   
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return Ok("Deleted");
    }
}
