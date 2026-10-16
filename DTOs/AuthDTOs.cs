namespace RaceDay_api.DTOs
{
    public class UserForRegistrationDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; } // organiser or participant
    }

    public class UserForLoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}