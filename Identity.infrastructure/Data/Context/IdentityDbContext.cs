using Identity.Domain.Entity;
using Identity.Domain.Entity.identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.infrastructure.Data.Context
{
    public class IdentityDbContext:IdentityDbContext<ApplicationUser,ApplicationRole,Guid>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<RefreshTokens> RefreshTokenss => Set<RefreshTokens>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();



        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUserRole<Guid>>(entity => entity.ToTable("UserRoles"));
            modelBuilder.Entity<IdentityUserClaim<Guid>>(entity => entity.ToTable("UserClaims"));
            modelBuilder.Entity<IdentityUserLogin<Guid>>(entity => entity.ToTable("UserLogins"));
            modelBuilder.Entity<IdentityRoleClaim<Guid>>(entity => entity.ToTable("RoleClaims"));
            modelBuilder.Entity<IdentityUserToken<Guid>>(entity => entity.ToTable("UserTokens"));

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly); 


        }
    }
  
}
