using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Domain.Entity.identity
{
    public class ApplicationUser: IdentityUser<Guid>
    {
        public string FullName { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public ICollection<RefreshTokens> RefreshTokens { get; set; }

        public ICollection<AuditLog> AuditLogs { get; set; }
    }
}
