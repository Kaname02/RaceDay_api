namespace RaceDay_api.Models
{
    public class Result
    {
        public int ResultId { get; set; }
        public int EnrolmentId { get; set; }
        public TimeSpan? FinishTime { get; set; }
        public int? Position { get; set; }
        public string Status { get; set; } // finished, dnf, dns
        public string? Notes { get; set; }

        public Enrolment Enrolment { get; set; }
    }
}