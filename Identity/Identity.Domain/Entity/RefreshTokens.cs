using Identity.Domain.Entity.identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Domain.Entity
{
    public class RefreshTokens : BaseEntity
    {
        public string Token { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsRevoked { get; set; }
        public string? CreatedByIp { get; set; }
        public string? UserAgent { get; set; }
        public ApplicationUser User { get; set; }
        public bool IsActive
        {
            get
            {
                return !IsRevoked && DateTime.Now < ExpirationDate;
            }
        }

    }
}
