using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Domain.Entity.identity
{
    public class ApplicationRole:IdentityRole<Guid>
    {
        public string Description { get; set; }

    }
}
