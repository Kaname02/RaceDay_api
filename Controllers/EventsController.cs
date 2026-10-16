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
    public class EventsController : ControllerBase
    {
        private readonly RaceDayContext _context;
        public EventsController(RaceDayContext context) => _context = context;
        /// <summary>
        /// Retrieves a list of all events in the system.
        /// </summary>
        /// <remarks>Accessible by both Organisers and Participants (requires authentication).</remarks>
        /// <returns>A list of all events, including their location details.</returns>
        [HttpGet]
        [Authorize] // Both roles can view
        public async Task<IActionResult> GetEvents() => Ok(await _context.Events.Include(e => e.Location).ToListAsync());
        /// <summary>
        /// Retrieves a specific event by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the event to retrieve.</param>
        /// <returns>The event details if found.</returns>
        /// <response code="200">Event found.</response>
        /// <response code="404">No event exists with the given ID.</response>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetEvent(int id)
        {
            var ev = await _context.Events.Include(e => e.Location).FirstOrDefaultAsync(e => e.EventId == id);
            return ev == null ? NotFound() : Ok(ev);
        }
        /// <summary>
        /// Creates a new event. Only accessible by Organisers.
        /// </summary>
        /// <param name="newEvent">The event details: Name, Description, Date, Location, Distance, and EventType (run/walk/cycle).</param>
        /// <returns>The created event with its assigned ID.</returns>
        /// <response code="200">Event created successfully.</response>
        /// <response code="403">Access denied. Only Organisers can create events.</response>
        [HttpPost]
        [Authorize(Roles = "organiser")] // Still keep this for real requests!
        public async Task<IActionResult> CreateEvent(Event newEvent)
        {
            // --- MANUAL CHECK for unit testing ---
            if (!User.IsInRole("organiser"))
            {
                return Forbid();
            }
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            newEvent.OrganiserId = userId;
            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();
            return Ok(newEvent);
        }
        /// <summary>
        /// Updates an existing event. Only the Organiser who created it can edit it.
        /// </summary>
        /// <param name="id">The ID of the event to update.</param>
        /// <param name="updatedEvent">The updated event information.</param>
        /// <response code="204">Event updated successfully.</response>
        /// <response code="403">Access denied. You can only edit your own events.</response>
        /// <response code="404">Event not found.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "organiser")]
        public async Task<IActionResult> UpdateEvent(int id, Event updatedEvent)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();
            if (ev.OrganiserId != userId) return Forbid("You can only edit your own events.");

            ev.Name = updatedEvent.Name;
            ev.Description = updatedEvent.Description;
            ev.Date = updatedEvent.Date;
            ev.EventType = updatedEvent.EventType;
            // update other fields as needed
            await _context.SaveChangesAsync();
            return NoContent();
        }
        /// <summary>
        /// Deletes an event. Only the Organiser who created it can delete it.
        /// </summary>
        /// <param name="id">The ID of the event to delete.</param>
        /// <response code="204">Event deleted successfully.</response>
        /// <response code="403">Access denied. You can only delete your own events.</response>
        /// <response code="404">Event not found.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "organiser")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();
            if (ev.OrganiserId != userId) return Forbid("You can only delete your own events.");

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}