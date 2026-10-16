namespace RaceDay_api.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public int OrganiserId { get; set; }
        public int LocationId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan? StartTime { get; set; }
        public DateTime? RegistrationDeadline { get; set; }
        public int? MaxParticipants { get; set; }
        public string Status { get; set; } = "draft";

        // REQUIRED BY REQUIREMENT 3:
        public string EventType { get; set; } // run, walk, or cycle

        public User Organiser { get; set; }
        public Location Location { get; set; }
        public ICollection<Category> Categories { get; set; }
        public ICollection<Enrolment> Enrolments { get; set; }
    }
}