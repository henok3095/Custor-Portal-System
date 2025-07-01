using CustorPortalAPI.Data;
using CustorPortalAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using File = CustorPortalAPI.Models.File;
using Microsoft.AspNetCore.Authorization;

namespace CustorPortalAPI.Controllers
{
    [Route("api/projects/{projectId}/Files")]
    [ApiController]
    [AllowAnonymous]
    public class FilesController : ControllerBase
    {
        private readonly CustorPortalDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public FilesController(CustorPortalDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }


        [HttpPost]
        public async Task<IActionResult> UploadFile(int projectId, [FromForm] IFormFile file, [FromForm] int uploaderKey, [FromForm] int version, [FromForm] bool? isCurrent)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Validate project existence
            var projectExists = await _context.Projects.AnyAsync(p => p.ProjectKey == projectId);
            if (!projectExists)
                return NotFound($"Project with ID {projectId} not found.");

            // Validate uploader existence
            var userExists = await _context.Users.AnyAsync(u => u.UserKey == uploaderKey);
            if (!userExists)
                return NotFound($"User with ID {uploaderKey} not found.");

            // Ensure uploads directory exists
            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            // Generate a unique file name to avoid conflicts
            var FileName = $"{Guid.NewGuid()}_{file.FileName}";
            var FilePath = Path.Combine(uploadsPath, FileName);

            // Save the file to the server
            using (var stream = new FileStream(FilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Create the File entity
            var fileEntity = new File
            {
                ProjectKey = projectId,
                FileName = file.FileName,
                FileType = file.ContentType,
                Version = version,
                FilePath = $"/uploads/{FileName}",
                Size = (int)file.Length,
                UploaderKey = uploaderKey,
                UploadedAt = DateTime.UtcNow,
                IsCurrent = isCurrent ?? true
            };

            _context.Files.Add(fileEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFiles), new { projectId, id = fileEntity.FileKey }, fileEntity);
        }
        [HttpGet]
        public async Task<IActionResult> GetFiles(int projectId)
        {
            var projectExists = await _context.Projects.AnyAsync(p => p.ProjectKey == projectId);
            if (!projectExists)
                return NotFound($"Project with ID {projectId} not found.");

            var files = await _context.Files
                .Where(f => f.ProjectKey == projectId)
                .Include(f => f.Uploader)
                .ThenInclude(u => u.Role)
                .Select(f => new
                {
                    f.FileKey,
                    f.ProjectKey,
                    f.FileName,
                    f.FileType,
                    f.Version,
                    f.FilePath,
                    f.Size,
                    f.UploaderKey,
                    f.UploadedAt,
                    f.IsCurrent,
                    Uploader = new
                    {
                        f.Uploader.UserKey,
                        f.Uploader.Email,
                        f.Uploader.First_Name,
                        f.Uploader.Last_Name,
                        Role = new
                        {
                            f.Uploader.Role.RoleKey,
                            f.Uploader.Role.Role_Name
                        }
                    }
                })
                .ToListAsync();

            return Ok(files);
        }

        [HttpPut("{fileId}")]
        public async Task<IActionResult> UpdateFile(int fileId, [FromBody] FileUpdateRequest request)
        {
            var file = await _context.Files.FindAsync(fileId);
            if (file == null)
                return NotFound($"File with ID {fileId} not found.");

            if (!string.IsNullOrWhiteSpace(request.FileName))
                file.FileName = request.FileName;
            if (!string.IsNullOrWhiteSpace(request.FilePath))
                file.FilePath = request.FilePath;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                file.FileKey,
                file.ProjectKey,
                file.FileName,
                file.FilePath,
                file.UploadedAt
            });
        }

        // New DELETE endpoint to delete a file
        [HttpDelete("{fileId}")]
        public async Task<IActionResult> DeleteFile(int fileId)
        {
            var file = await _context.Files
                .Include(f => f.Comments)
                .FirstOrDefaultAsync(f => f.FileKey == fileId);
            if (file == null)
                return NotFound($"File with ID {fileId} not found.");

            if (file.Comments.Any())
                return BadRequest("Cannot delete file with existing comments.");

            _context.Files.Remove(file);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class FileUploadRequest
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }

    public class FileUpdateRequest
    {
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
    }

}


