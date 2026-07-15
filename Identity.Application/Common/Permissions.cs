namespace Identity.Application.Common
{
  
    public static class Permissions
    {
        public const string UsersView = "users.view";
        public const string UsersCreate = "users.create";
        public const string UsersEdit = "users.edit";
        public const string UsersDelete = "users.delete";
        public const string UsersLock = "users.lock";

        public const string RolesManage = "roles.manage";
        public const string PermissionsManage = "permissions.manage";

        public const string AuditLogsView = "auditlogs.view";

        public static readonly IReadOnlyList<string> All = new[]
        {
            UsersView, UsersCreate, UsersEdit, UsersDelete, UsersLock,
            RolesManage, PermissionsManage, AuditLogsView
        };
    }
}
