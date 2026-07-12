using Identity.Domain.Entity.identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Domain.Entity
{
    public class AuditLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? UserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public DateTime DateTime { get; set; } = DateTime.Now;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Details { get; set; }
        public ApplicationUser User { get; set; }
    }
}
