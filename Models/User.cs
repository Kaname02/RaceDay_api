using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace RaceDay_api.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Role { get; set; } // "organiser" or "participant"

        // Add this navigation property to fix CS1061
        public ICollection<Event> OrganisedEvents { get; set; }
        public ICollection<Enrolment> Enrolments { get; set; } // Add this property to fix CS1061
    }
}