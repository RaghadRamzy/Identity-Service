namespace Identity.Application.DTOs.Users
{
    public class UpdateUserRolesRequest
    {
        public List<string> Roles { get; set; } = new();
    }
}
