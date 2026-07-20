using Identity.Domain.Entity.identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Identity.infrastructure.Data.Seed
{
    /// <summary>
    /// Runs once at startup. Guarantees the roles the system depends on exist, and - only if
    /// credentials are supplied via configuration - creates a first Admin user so there is a way
    /// to call the Admin-only endpoints on a brand new database. No admin password is ever
    /// hardcoded here; if AdminSeed:Password isn't configured, admin bootstrapping is skipped.
    /// </summary>
    public static class IdentitySeeder
    {
        private static readonly string[] DefaultRoles = { "Admin", "Customer" };

        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("IdentitySeeder");

            foreach (var roleName in DefaultRoles)
            {
                if (await roleManager.RoleExistsAsync(roleName))
                    continue;

                var result = await roleManager.CreateAsync(new ApplicationRole
                {
                    Name = roleName,
                    Description = $"{roleName} role (seeded on startup)"
                });

                if (result.Succeeded)
                    logger.LogInformation("Seeded default role '{RoleName}'.", roleName);
                else
                    logger.LogWarning("Failed to seed default role '{RoleName}': {Errors}",
                        roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await SeedAdminUserAsync(userManager, configuration, logger);
        }

        private static async Task SeedAdminUserAsync(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            ILogger logger)
        {
            var adminSection = configuration.GetSection("AdminSeed");
            var email = adminSection["Email"];
            var username = adminSection["Username"];
            var password = adminSection["Password"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                logger.LogWarning(
                    "AdminSeed:Email / Username / Password are not fully configured - skipping bootstrap admin user. " +
                    "Set them (e.g. via user-secrets or environment variables) if you need one created on startup.");
                return;
            }

            if (await userManager.FindByEmailAsync(email) is not null)
                return; // already seeded on a previous run

            var adminUser = new ApplicationUser
            {
                UserName = username,
                Email = email,
                FullName = adminSection["FullName"] ?? "System Administrator",
                PhoneNumber = adminSection["PhoneNumber"] ?? "0000000000",
                IsActive = true,
                EmailConfirmed = true,
                CreatedDate = DateTime.UtcNow
            };

            var createResult = await userManager.CreateAsync(adminUser, password);
            if (!createResult.Succeeded)
            {
                logger.LogError("Failed to seed bootstrap admin user: {Errors}",
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
                return;
            }

            await userManager.AddToRoleAsync(adminUser, "Admin");
            logger.LogInformation("Seeded bootstrap admin user '{Username}'.", username);
        }
    }
}
