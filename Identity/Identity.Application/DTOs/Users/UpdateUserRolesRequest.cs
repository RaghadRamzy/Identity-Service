namespace Identity.Application.DTOs.Users
{
    // Replaces the user's current role assignments with this list.
    public class UpdateUserRolesRequest
    {
        public List<string> Roles { get; set; } = new();
    }
}
