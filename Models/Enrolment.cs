namespace RaceDay_api.Models
{
    public class Enrolment
    {
        public int EnrolmentId { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public int CategoryId { get; set; }
        public DateTime EnrolmentDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "pending";
        public string PaymentStatus { get; set; } = "unpaid";
        public int? BibNumber { get; set; }

        public User User { get; set; }
        public Event Event { get; set; }
        public Category Category { get; set; }
        public Result? Result { get; set; }
    }
}