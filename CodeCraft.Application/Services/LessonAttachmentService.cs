using CodeCraft.Application.DTOs.Attachment;
using CodeCraft.Application.Interfaces.Repositories;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.Services;
public class LessonAttachmentService: ILessonAttachmentService
{
    private readonly  IGenericRepository<LessonAttachment> _repo;
    private readonly IMapper _Mapper;

    public LessonAttachmentService(IGenericRepository<LessonAttachment> repo, IMapper Mapper)
    {
        _repo = repo;
        _Mapper=Mapper;
    }

    public async Task<IEnumerable<AttachmentDto>> GetByLessonIdAsync(int lessonId)
    {
        var attachments = await _repo.FindAsync(a => a.LessonId == lessonId);

        return attachments.Select(a => new AttachmentDto
        {
            Id = a.Id,
            FileName = a.FileName,
            FileUrl = a.FileUrl
        });
    }

    public async Task<AttachmentDto> UploadAsync(CreateAttachmentDto dto)
    {
        if (dto.FileStream == null || dto.FileStream.Length == 0)
            throw new Exception("File is empty");

        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".ppt", ".pptx" };

        var extension = Path.GetExtension(dto.FileName).ToLower();

        if (!allowedExtensions.Contains(extension))
            throw new Exception("Invalid file type");

        var maxSize = 50 * 1024 * 1024;

        if (dto.FileStream.Length > maxSize)
            throw new Exception("File size exceeds limit (50MB)");

        var fileName = Guid.NewGuid() + extension;

        var path = Path.Combine("wwwroot/attachments", fileName);

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await dto.FileStream.CopyToAsync(stream);
        }

        var attachment = new LessonAttachment
        {
            FileName = dto.FileName,
            FileUrl = $"/attachments/{fileName}",
            LessonId = dto.LessonId
        };

        await _repo.AddAsync(attachment);

        return new AttachmentDto
        {
            Id = attachment.Id,
            FileName = attachment.FileName,
            FileUrl = attachment.FileUrl
        };
    }

    public async Task DeleteAsync(int id)
    {
        var attachment = await _repo.GetByIdAsync(id);

        if (attachment == null)
            throw new Exception("Attachment not found");

        await _repo.DeleteAsync(attachment);
        
    }

}
