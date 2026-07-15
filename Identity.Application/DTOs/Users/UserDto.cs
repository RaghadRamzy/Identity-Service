namespace Identity.Application.DTOs.Users
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
        public DateTime CreatedDate { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
