using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Domain.Entity
{
    public class Permission : BaseEntity
    {
        public string Name { get; set; } 
        public string? Description { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    }
}
