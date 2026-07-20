namespace Identity.Application.DTOs.Permissions
{
    // FR-07: replaces the target's current permission claims with this list.
    public class AssignPermissionsRequest
    {
        public List<string> Permissions { get; set; } = new();
    }
}
