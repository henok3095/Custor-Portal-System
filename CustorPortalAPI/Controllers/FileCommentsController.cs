using CustorPortalAPI.Data;
using CustorPortalAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CustorPortalAPI.Controllers
{
    [Route("api/files/{fileId:int}/comments")]
    [ApiController]
    [AllowAnonymous]
    public class FileCommentsController : ControllerBase
    {
        private readonly CustorPortalDbContext _context;

        public FileCommentsController(CustorPortalDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(int fileId, [FromBody] CommentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                return BadRequest("Comment cannot be empty");

            // Validate FileId
            var file = await _context.Files.FindAsync(fileId);
            if (file == null)
                return NotFound($"File with ID {fileId} not found.");

            // Validate UserId
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
                return NotFound($"User with ID {request.UserId} not found.");

            var comment = new Comment
            {
                DocumentId = fileId,
                Text = request.Text,
                UserId = request.UserId,
                Timestamp = DateTime.UtcNow, // Fix: Use 'Timestamp' instead of 'CreatedAt'
                Mentions = request.Mentions != null && request.Mentions.Any()
                    ? JsonSerializer.Serialize(request.Mentions)
                    : null
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Handle mentions and notifications
            if (request.Mentions != null && request.Mentions.Any())
            {
                foreach (var mention in request.Mentions)
                {
                    var mentionedUser = await _context.Users
                        .FirstOrDefaultAsync(u => (u.First_Name + " " + u.Last_Name).ToLower() == mention.ToLower());
                    if (mentionedUser != null)
                    {
                        _context.Notifications.Add(new Notification
                        {
                            UserId = mentionedUser.UserKey,
                            Message = $"{user.First_Name} {user.Last_Name} mentioned you in a comment on file {file.FileName}",
                            Link = $"/files/{fileId}",
                            Timestamp = DateTime.UtcNow
                        });
                    }
                }
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetComments), new { fileId = fileId }, new
            {
                id = comment.Id,
                text = comment.Text,
                userId = comment.UserId,
                timestamp = comment.Timestamp, // Fix: Use 'Timestamp' instead of 'CreatedAt'
                mentions = comment.Mentions
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetComments(int fileId)
        {
            var file = await _context.Files.FindAsync(fileId);
            if (file == null)
                return NotFound($"File with ID {fileId} not found.");

            var comments = await _context.Comments
                .Where(c => c.DocumentId == fileId)
                .Include(c => c.User)
                .ThenInclude(u => u.Role)
                .Select(c => new
                {
                    c.Id,
                    Text = c.Text,
                    c.UserId,
                    Timestamp = c.Timestamp, // Fix: Use 'Timestamp' instead of 'CreatedAt'
                    c.Mentions,
                    User = new
                    {
                        c.User.UserKey,
                        c.User.Email,
                        c.User.First_Name,
                        c.User.Last_Name,
                        Role = new
                        {
                            c.User.Role.RoleKey,
                            c.User.Role.Role_Name
                        }
                    }
                })
                .ToListAsync();

            return Ok(comments);
        }
        // Fix the incorrect property name 'CommentText' to 'Text' in the UpdateFileComment method.
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFileComment(int fileId, int id, [FromBody] CommentRequest request)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == id && c.DocumentId == fileId);
            if (comment == null)
                return NotFound($"Comment with ID {id} for File {fileId} not found.");

            if (string.IsNullOrWhiteSpace(request.Text))
                return BadRequest("Comment text cannot be empty.");

            // Corrected property name from 'CommentText' to 'Text'
            comment.Text = request.Text;
            comment.Mentions = request.Mentions != null && request.Mentions.Any()
                ? JsonSerializer.Serialize(request.Mentions)
                : null;
            comment.Timestamp = DateTime.UtcNow; // Assuming 'Timestamp' is used for updates

            // Update notifications for new mentions (if any)
            if (request.Mentions != null && request.Mentions.Any())
            {
                foreach (var mention in request.Mentions)
                {
                    var mentionedUser = await _context.Users
                        .FirstOrDefaultAsync(u => (u.First_Name + " " + u.Last_Name).ToLower() == mention.ToLower());
                    if (mentionedUser != null)
                    {
                        _context.Notifications.Add(new Notification
                        {
                            UserId = mentionedUser.UserKey,
                            Message = $"A comment on file was updated with a mention of you by User {comment.UserId}",
                            Link = $"/files/{fileId}",
                            Timestamp = DateTime.UtcNow,
                            Read = false
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                comment.Id,
                text = comment.Text, // Corrected property name
                comment.UserId,
                timestamp = comment.Timestamp, // Assuming 'Timestamp' is used for updates
                comment.Mentions
            });
        }

        // New DELETE endpoint to delete a file comment
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFileComment(int fileId, int id)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == id && c.DocumentId == fileId);
            if (comment == null)
                return NotFound($"Comment with ID {id} for File {fileId} not found.");

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}