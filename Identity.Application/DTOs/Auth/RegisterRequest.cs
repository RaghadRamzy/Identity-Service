using System.ComponentModel.DataAnnotations;
namespace Identity.Application.DTOs.Auth
{
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Phone number must be a valid Egyptian mobile number (e.g. 01012345678).")]
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
