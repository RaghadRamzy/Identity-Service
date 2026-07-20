namespace Identity.Application.DTOs.Permissions
{
    public class AssignPermissionsRequest
    {
        public List<string> Permissions { get; set; } = new();
    }
}
