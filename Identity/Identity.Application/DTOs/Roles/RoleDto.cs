namespace Identity.Application.DTOs.Roles
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IList<string> Permissions { get; set; } = new List<string>();
    }
}
