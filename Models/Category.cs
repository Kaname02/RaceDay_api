using System.ComponentModel.DataAnnotations.Schema;
namespace RaceDay_api.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public int EventId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal DistanceKm { get; set; }

        public TimeSpan? StartTime { get; set; }

        [Column(TypeName = "decimal(10,2)")] //
        public decimal? Fee { get; set; }

        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }

        public Event Event { get; set; }
        public ICollection<Enrolment> Enrolments { get; set; }
    }
}