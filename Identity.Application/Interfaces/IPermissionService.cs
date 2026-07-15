namespace Identity.Application.Interfaces
{

    public interface IPermissionService
    {
        Task<IList<string>> GetRolePermissionsAsync(Guid roleId);
        Task AssignPermissionsToRoleAsync(Guid roleId, IEnumerable<string> permissions);

        Task<IList<string>> GetUserDirectPermissionsAsync(Guid userId);
        Task AssignPermissionsToUserAsync(Guid userId, IEnumerable<string> permissions);

        Task<IList<string>> GetEffectivePermissionsForUserAsync(Guid userId);
    }
}
