using CustorPortalAPI.Data;
using CustorPortalAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustorPortalAPI.Controllers
{
    [Route("api/users/{userId:int}/notifications")]
    [ApiController]
    [AllowAnonymous]
    public class NotificationsController : ControllerBase
    {
        private readonly CustorPortalDbContext _context;

        public NotificationsController(CustorPortalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications(int userId)
        {
            if (userId <= 0)
                return BadRequest("Invalid UserId");

            var userExists = await _context.Users.AnyAsync(u => u.UserKey == userId);
            if (!userExists)
                return NotFound($"User with ID {userId} not found.");

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .Include(n => n.User)
                .ThenInclude(u => u.Role)
                .Select(n => new
                {
                    n.Id,
                    n.Message,
                    n.Link,
                    IsRead = n.Read, // Fixed the property name
                    n.Timestamp,
                    User = new
                    {
                        n.User.UserKey,
                        n.User.Email,
                        n.User.First_Name,
                        n.User.Last_Name,
                        Role = new
                        {
                            n.User.Role.RoleKey,
                            n.User.Role.Role_Name
                        }
                    }
                })
                .ToListAsync();

            return Ok(notifications);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> MarkAsRead(int userId, int id)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
            if (notification == null)
                return NotFound($"Notification with ID {id} for User {userId} not found.");

            notification.Read = true;
            await _context.SaveChangesAsync();

            return Ok(new { id = notification.Id, isRead = notification.Read });
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotification(int userId, [FromBody] NotificationCreateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message) || string.IsNullOrWhiteSpace(request.Link))
                return BadRequest("Message and Link are required.");

            var userExists = await _context.Users.AnyAsync(u => u.UserKey == userId);
            if (!userExists)
                return NotFound($"User with ID {userId} not found.");

            var notification = new Notification
            {
                UserId = userId,
                Message = request.Message,
                Link = request.Link,
                Timestamp = DateTime.UtcNow,
                Read = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNotifications), new { userId = userId }, new
            {
                notification.Id,
                notification.Message,
                notification.Link,
                notification.Read,
                notification.Timestamp
            });
        }
    }

    public class NotificationCreateRequest
    {
        public string Message { get; set; }
        public string Link { get; set; }
    }
}