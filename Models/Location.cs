using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_api.Models
{
    public class Location
    {
        public int LocationId { get; set; }
        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string? PostalCode { get; set; }
        public string Country { get; set; } = "South Africa";

        [Column(TypeName = "decimal(10,8)")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal(11,8)")] // <-- Add this
        public decimal? Longitude { get; set; }

        public ICollection<Event> Events { get; set; }
    }
}