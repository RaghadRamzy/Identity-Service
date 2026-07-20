using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Domain.Enum
{
    public enum AuditAction
    {
        Login,
        Logout,
        UserCreated,
        UserUpdated,
        UserDeleted,
        PasswordChanged,
        PasswordReset,
        PermissionsChanged,
        RoleCreated,
        RoleUpdated,
        RoleDeleted,
        AccountLocked,
        AccountUnlocked,
        UserRolesUpdated
    }
}
