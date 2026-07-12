using Identity.Domain.Entity.identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Domain.Entity
{
    public class RolePermission
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; } = null!;
        public ApplicationRole Role { get; set; }
    }
}
