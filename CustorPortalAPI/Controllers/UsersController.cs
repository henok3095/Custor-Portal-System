using CustorPortalAPI.Data;
using CustorPortalAPI.Helpers;
using CustorPortalAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginRequest = CustorPortalAPI.Models.LoginRequest;

namespace CustorPortalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly CustorPortalDbContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(CustorPortalDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email and Password are required.");

            var user=await _context.Users
                .Include(u=>u.Role)
                .FirstOrDefaultAsync(u=>u.Email.ToLower()==request.Email.ToLower());

            if (user == null)
                return Unauthorized("Invalid email or password.");

            if (request.Password != "Password")
                return Unauthorized("Invalid email or password.");

            var token = JwtHelper.GenerateJwtToken(user.UserKey, user.Email, _configuration);

            return Ok(new
                
               {
                Token= token,
                User = new 
                {
                    user.UserKey,
                    user.Email,
                    Role=user.Role.Role_Name
                }
            });
        }
        [HttpPost("register")]
        // [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email and Password are required.");

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());
            if (existingUser != null)
                return BadRequest("User already exists.");

            var user = new User
            {
                Email = request.Email,
                Password_Hash = request.Password, // Corrected property name
                // Set other properties as needed
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully." });
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Select(u => new
                {
                    u.UserKey,
                    u.Email,
                    Role = u.Role.Role_Name
                })
                .ToListAsync();

            return Ok(users); // This should return a plain array
        }

    }
}
