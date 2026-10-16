using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_api.Data;
using System.Security.Claims;

namespace RaceDay_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Both roles must be logged in
    public class UserProfileController : ControllerBase
    {
        private readonly RaceDayContext _context;
        public UserProfileController(RaceDayContext context) => _context = context;

        /// <summary>
        /// Retrieves the currently logged-in user's profile information.
        /// </summary>
        /// <returns>The user's ID, full name, email, and role.</returns>
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => new { u.UserId, u.FullName, u.Email, u.Role })
                .FirstOrDefaultAsync();
            return Ok(user);
        }
        /// <summary>
        /// Updates the currently logged-in user's profile information.
        /// </summary>
        /// <param name="fullname">New full name (optional).</param>
        /// <param name="email">New email (optional).</param>
        /// <param name="password">New password (optional, will be hashed).</param>
        /// <response code="204">Profile updated successfully.</response>
        /// <response code="404">User not found.</response>
        [HttpPut]
        public async Task<IActionResult> UpdateProfile(string fullname, string email, string? password)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            if (!string.IsNullOrEmpty(fullname)) user.FullName = fullname;
            if (!string.IsNullOrEmpty(email)) user.Email = email;
            if (!string.IsNullOrEmpty(password)) user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}