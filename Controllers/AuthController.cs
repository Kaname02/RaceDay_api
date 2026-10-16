using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RaceDay_api.Data;
using RaceDay_api.DTOs;
using RaceDay_api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RaceDay_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RaceDayContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(RaceDayContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Registers a new user account as either an Organiser or a Participant.
        /// </summary>
        /// <param name="request">The user's email, password, full name, and role.</param>
        /// <returns>A success message confirming the user was created.</returns>
        /// <response code="200">User registered successfully.</response>
        /// <response code="400">Email already exists or role is invalid.</response>
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegistrationDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest("Email already exists.");

            if (request.Role.ToLower() != "organiser" && request.Role.ToLower() != "participant")
                return BadRequest("Role must be 'organiser' or 'participant'.");

            var user = new User
            {
                Email = request.Email,
                FullName = request.FullName,
                Role = request.Role.ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password) // Hash password!
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }
        /// <summary>
        /// Authenticates a user and returns a JWT token for subsequent requests.
        /// </summary>
        /// <param name="request">The user's email and password.</param>
        /// <returns>A JWT token, the user's role, and full name.</returns>
        /// <response code="200">Login successful. Returns JWT token.</response>
        /// <response code="401">Invalid email or password.</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) return Unauthorized("Invalid email or password.");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized("Invalid email or password.");

            var token = CreateToken(user);
            return Ok(new { Token = token, Role = user.Role, FullName = user.FullName });
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}