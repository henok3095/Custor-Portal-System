using CustorPortalAPI.Data;
using CustorPortalAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustorPortalAPI.Controllers
{
    [Route("api/tasks/{taskId}/assignees")]
    [ApiController]
    [AllowAnonymous]
    public class TaskAssigneesController : ControllerBase
    {
        private readonly CustorPortalDbContext _context;

        public TaskAssigneesController(CustorPortalDbContext context)
        {
            _context = context;
        }

       

        [HttpPost]
        public async Task<IActionResult> AssignUser(int taskId, [FromBody] TaskAssigneeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Ensure the task exists
            var taskExists = await _context.Tasks.AnyAsync(t => t.TaskKey == taskId);
            if (!taskExists)
                return NotFound($"Task with ID {taskId} not found.");

            // Ensure the user exists
            var userExists = await _context.Users.AnyAsync(u => u.UserKey == request.UserKey);
            if (!userExists)
                return NotFound($"User with ID {request.UserKey} not found.");

            // Check if the user is already assigned to the task
            var alreadyAssigned = await _context.TaskAssignees
                .AnyAsync(ta => ta.Taskkey == taskId && ta.UserKey == request.UserKey);
            if (alreadyAssigned)
                return Conflict($"User with ID {request.UserKey} is already assigned to Task with ID {taskId}.");

            var taskAssignee = new TaskAssignee
            {
                Taskkey = taskId,
                UserKey = request.UserKey
            };
            _context.TaskAssignees.Add(taskAssignee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTasks), new { taskId }, new
            {
                taskAssignee.Taskkey,
                taskAssignee.UserKey
            });
                
        }
       [HttpGet]
public async Task<IActionResult> GetTasks()
{
    var tasks = await _context.Tasks
        .Include(t => t.TaskAssignees)
            .ThenInclude(ta => ta.User)
        .Select(t => new {
            t.TaskKey,
            t.Title,
            t.Description,
            t.Status,
            t.Priority,
            t.Deadline,
            // ... other fields ...
            Assignee = t.TaskAssignees.Select(ta => ta.User.Email).FirstOrDefault() // or .ToList() for multiple
        })
        .ToListAsync();

    return Ok(tasks);
}

        [HttpDelete("{userId}")]
        public async Task<IActionResult> UnassignUserFromTask(int taskId, int userId)
        {
            var taskAssignee = await _context.TaskAssignees
                .FirstOrDefaultAsync(ta => ta.Taskkey == taskId && ta.UserKey == userId);
            if (taskAssignee == null)
                return NotFound($"User {userId} is not assigned to Task {taskId}.");

            _context.TaskAssignees.Remove(taskAssignee);

            // Notify the user of unassignment
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.TaskKey == taskId);
            var notification = new Notification
            {
                UserId = userId,
                Message = $"You have been unassigned from task '{task.Title} ' in project ' {task.Project.Name}'",
                Link = $"/tasks/{taskId}",
                Timestamp = DateTime.UtcNow,
                Read = false
            };
            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class TaskAssigneeRequest
    {
        public int UserKey { get; set; }
    }
}