namespace Identity.Application.DTOs.Auth
{

    public class TokenRequest
    {
        public string GrantType { get; set; } = string.Empty; 
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? RefreshToken { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? Scope { get; set; }
    }
}
