using CustorPortalAPI.Data;
using Microsoft.EntityFrameworkCore;
using CustorPortalAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CustorPortalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ProjectsController : ControllerBase
    {
        private readonly CustorPortalDbContext _context;
        public ProjectsController(CustorPortalDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // project.Created_at = DateTime.UtcNow;
            if (!await _context.Users.AnyAsync(u => u.UserKey == request.creatorKey))
                return NotFound($"User with ID {request.creatorKey} not found.");

            var project = new Project
            {
                Name = request.Name,
                Description = request.Description,
                Created_at = DateTime.UtcNow,
                Updated_at = null,
                creatorKey = request.creatorKey
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProject), new { projectId = project.ProjectKey }, new
            {
                project.ProjectKey,
                project.Name,
                project.Description,
                project.Created_at,
                project.Updated_at,
                CreatorKey = project.creatorKey
            }
                );
        }
        [HttpGet("{projectId}")]
        public async Task<ActionResult> GetProject(int projectId)
        {
            var project = await _context.Projects
                .Include(p => p.Creator)
                .FirstOrDefaultAsync(p => p.ProjectKey == projectId);
            if (project == null)
                return NotFound($"Project with ID {projectId} not found.");

            return Ok(project);
        }
        [HttpPut("{projectId}")]
        public async Task<IActionResult> UpdateProject(int projectId, [FromBody] ProjectUpdateRequest request)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
                return NotFound($"Project with ID {projectId} not found.");

            if (!string.IsNullOrWhiteSpace(request.Name))
                project.Name = request.Name;
            if (!string.IsNullOrWhiteSpace(request.Description))
                project.Description = request.Description;
            project.Updated_at = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                project.ProjectKey,
                project.Name,
                project.Description,
                project.Created_at,
                project.Updated_at
            });
        }

        // New DELETE endpoint to delete a project
        [HttpDelete("{projectId}")]
        public async Task<IActionResult> DeleteProject(int projectId)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .Include(p => p.Files)
                .Include(p => p.UserProjects)
                .FirstOrDefaultAsync(p => p.ProjectKey == projectId);
            if (project == null)
                return NotFound($"Project with ID {projectId} not found.");

            if (project.Tasks.Any())
                return BadRequest("Cannot delete project with existing tasks.");
            if (project.Files.Any())
                return BadRequest("Cannot delete project with existing files.");
            if (project.UserProjects.Any())
                return BadRequest("Cannot delete project with assigned users.");

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class ProjectCreateRequest
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int creatorKey { get; set; }
    }

    public class ProjectUpdateRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}