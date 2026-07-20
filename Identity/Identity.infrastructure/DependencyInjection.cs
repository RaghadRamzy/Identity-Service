using Identity.Application;
using Identity.Application.Interfaces;
using Identity.Domain.Entity.identity;
using Identity.infrastructure.Data.Context;
using Identity.infrastructure.Repositories;
using Identity.infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;


namespace Identity.infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true; 
            })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

            services.AddHttpContextAccessor();

            services.AddScoped<IIdentityService, IdentityServiceImpl>();
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IPermissionService, PermissionServiceImpl>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IClientValidator, ClientValidator>();

            services.AddHostedService<RefreshTokenCleanupService>();

            services.AddApplication();

            return services;
        }
    }
}
