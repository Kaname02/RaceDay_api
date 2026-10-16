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
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayContext _context;
        public ResultsController(RaceDayContext context) => _context = context;
        /// <summary>
        /// Captures the result (finish time and position) for a Participant's enrolment. Only for Organisers.
        /// </summary>
        /// <param name="enrolmentId">The ID of the enrolment being finalised.</param>
        /// <param name="finishTime">The participant's finish time.</param>
        /// <param name="position">The participant's finishing position.</param>
        /// <returns>The created result entry.</returns>
        [HttpPost]
        [Authorize(Roles = "organiser")]
        public async Task<IActionResult> CaptureResult(int enrolmentId, TimeSpan finishTime, int position)
        {
            var result = new Result
            {
                EnrolmentId = enrolmentId,
                FinishTime = finishTime,
                Position = position,
                Status = "finished"
            };
            _context.Results.Add(result);
            await _context.SaveChangesAsync();
            return Ok(result);
        }
        /// <summary>
        /// Retrieves all results for the currently logged-in Participant.
        /// </summary>
        /// <returns>A list of results, including event details.</returns>
        [HttpGet("my-results")]
        [Authorize(Roles = "participant")]
        public async Task<IActionResult> GetMyResults()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var results = await _context.Results
                .Include(r => r.Enrolment)
                .ThenInclude(e => e.Event)
                .Where(r => r.Enrolment.UserId == userId)
                .ToListAsync();
            return Ok(results);
        }
    }
}