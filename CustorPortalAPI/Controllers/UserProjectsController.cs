using CustorPortalAPI.Data;
using CustorPortalAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustorPortalAPI.Controllers
{
    [Route("api/projects/{projectId:int}/users")]
    [ApiController]
    [AllowAnonymous]
    public class UserProjectsController : ControllerBase
    {
        private readonly CustorPortalDbContext _context;

        public UserProjectsController(CustorPortalDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AssignUserToProject(int projectId, [FromBody] UserProjectRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Role))
                return BadRequest("Role is required");

            // Validate ProjectId
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
                return NotFound($"Project with ID {projectId} not found.");

            // Validate UserId
            var user = await _context.Users.FindAsync(request.UserKey);
            if (user == null)
                return NotFound($"User with ID {request.UserKey} not found.");

            // Check if the user is already assigned to the project
            var existingAssignment = await _context.UserProjects
                .AnyAsync(up => up.UserKey == request.UserKey && up.ProjectKey == projectId);
            if (existingAssignment)
                return BadRequest($"User with ID {request.UserKey} is already assigned to Project {projectId}.");

            var userProject = new UserProject
            {
                UserKey = request.UserKey,
                ProjectKey = projectId,
                Role = request.Role,
                Assigned_at = DateTime.UtcNow
            };

            _context.UserProjects.Add(userProject);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsersForProject), new { projectId = projectId }, new
            {
                userKey = userProject.UserKey,
                projectKey = userProject.ProjectKey,
                role = userProject.Role,
                assignedAt = userProject.Assigned_at
            });
        }

        [HttpGet]
        
     
        public async Task<IActionResult> GetUsersForProject(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
                return NotFound($"Project with ID {projectId} not found.");

            var users = await _context.UserProjects
                .Where(up => up.ProjectKey == projectId)
                .Include(up => up.User)
                .ThenInclude(u => u.Role)
                .Select(up => new
                {
                    userKey = up.UserKey,
                    projectKey = up.ProjectKey,
                    role = up.Role,
                   assignedAt = up.Assigned_at,
                    User = new
                    {
                        up.User.UserKey,
                        up.User.Email,
                        up.User.First_Name,
                        up.User.Last_Name,
                        Role = new
                        {
                            up.User.Role.RoleKey,
                            up.User.Role.Role_Name
                        }
                    }
                })
                .ToListAsync();

            return Ok(users);
        }
    
    [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUserRoleInProject(int projectId, int userId, [FromBody] UserProjectRequest request)
        {
            var userProject = await _context.UserProjects
                .FirstOrDefaultAsync(up => up.ProjectKey == projectId && up.UserKey == userId);
            if (userProject == null)
                return NotFound($"User {userId} not found in Project {projectId}.");

            if (string.IsNullOrWhiteSpace(request.Role))
                return BadRequest("Role is required.");

            userProject.Role = request.Role;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                userKey = userProject.UserKey,
                projectKey = userProject.ProjectKey,
                role = userProject.Role,
                assigned_at = userProject.Assigned_at
            });
        }

        // New DELETE endpoint to remove user from project
        [HttpDelete("{userId}")]
        public async Task<IActionResult> RemoveUserFromProject(int projectId, int userId)
        {
            var userProject = await _context.UserProjects
                .FirstOrDefaultAsync(up => up.ProjectKey == projectId && up.UserKey == userId);
            if (userProject == null)
                return NotFound($"User {userId} not found in Project {projectId}.");

            // Check if the user is assigned to tasks in this project
            var taskAssignments = await _context.TaskAssignees
                .Include(ta => ta.Task)
                .Where(ta => ta.UserKey == userId && ta.Task.ProjectKey == projectId)
                .ToListAsync();
            if (taskAssignments.Any())
                return BadRequest("Cannot remove user from project because they are assigned to tasks.");

            _context.UserProjects.Remove(userProject);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }




    public class UserProjectRequest
    {
        public int UserKey { get; set; }
        public string? Role { get; set; }
    }
}