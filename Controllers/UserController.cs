using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_api.Data;
using RaceDay_api.Models;

namespace RaceDay_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public UserController(RaceDayContext context)
        {
            _context = context;
        }

        // POST: api/User
        [HttpPost]
        public async Task<IActionResult> CreateUser(string email, string password, string fullname, string role)
        {
            if (string.IsNullOrWhiteSpace(role) ||
                (!role.Equals("organiser", StringComparison.OrdinalIgnoreCase) &&
                 !role.Equals("participant", StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest("Role must be 'organiser' or 'participant'.");
            }

            // NOTE: In a real app, you MUST hash the password here using BCrypt!
            var newUser = new User
            {
                Email = email,
                PasswordHash = password, // Placeholder: hash this later!
                FullName = fullname,
                Role = role.ToLower()
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok("User info saved successfully.");
        }

        // GET: api/User/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users
                .Where(u => u.UserId == id)
                .Select(u => new
                {
                    Id = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/User/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, string? fullname, string? email, string? password, string? role)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (fullname != null) user.FullName = fullname;
            if (email != null) user.Email = email;
            if (password != null) user.PasswordHash = password; // Hash this in production!

            if (role != null)
            {
                var r = role.Trim().ToLower();
                if (r != "organiser" && r != "participant") return BadRequest("Invalid role.");
                user.Role = r;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/User/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}