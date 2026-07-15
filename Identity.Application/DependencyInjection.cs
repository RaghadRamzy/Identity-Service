using Identity.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<AuthService>();
            services.AddScoped<UserService>();
            services.AddScoped<RoleService>();
            services.AddScoped<PermissionService>();
            return services;
        }
    }
}
