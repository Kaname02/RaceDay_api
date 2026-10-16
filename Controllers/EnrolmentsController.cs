using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_api.Data;
using RaceDay_api.Models;
using System.Security.Claims;

namespace RaceDay_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayContext _context;
        public EnrolmentsController(RaceDayContext context) => _context = context;
        /// <summary>
        /// Enrols the currently logged-in Participant into an event category.
        /// </summary>
        /// <param name="eventId">The ID of the event to enrol in.</param>
        /// <param name="categoryId">The ID of the category within that event.</param>
        /// <returns>A confirmation message.</returns>
        /// <response code="200">Enrolled successfully.</response>
        /// <response code="403">Access denied. Only Participants can enrol.</response>
        [HttpPost]
        [Authorize(Roles = "participant")] // ONLY participants
        public async Task<IActionResult> Enrol(int eventId, int categoryId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var enrolment = new Enrolment
            {
                UserId = userId,
                EventId = eventId,
                CategoryId = categoryId,
                Status = "pending",
                PaymentStatus = "unpaid"
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();
            return Ok("Enrolled successfully.");
        }
        /// <summary>
        /// Views all enrolments for a specific event. Only the owning Organiser can view them.
        /// </summary>
        /// <param name="eventId">The ID of the event.</param>
        /// <returns>A list of enrolments with user and category details.</returns>
        /// <response code="200">Enrolments returned successfully.</response>
        /// <response code="403">Access denied. You can only view enrolments for your own events.</response>
        [HttpGet("event/{eventId}")]
        [Authorize(Roles = "organiser")] // ONLY organisers
        public async Task<IActionResult> GetEventEnrolments(int eventId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null || ev.OrganiserId != userId) return Forbid("Not your event.");

            var enrolments = await _context.Enrolments
                .Where(e => e.EventId == eventId)
                .Include(e => e.User)
                .Include(e => e.Category)
                .ToListAsync();

            return Ok(enrolments);
        }
    }
}