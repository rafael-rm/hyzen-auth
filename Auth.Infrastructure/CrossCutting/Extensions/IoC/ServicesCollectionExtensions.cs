using Auth.Application.Interfaces.Application;
using Auth.Application.Interfaces.Infrastructure;
using Auth.Application.Services;
using Auth.Infrastructure.Data;
using Auth.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Infrastructure.CrossCutting.Extensions.IoC;

public static class ServicesCollectionExtensions
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRoleService, RoleService>();
    }
        
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IHashService, Pbkdf2HashService>();
        services.AddSingleton<ITokenService, JwtService>();

        services.AddScoped<IAuthDbContext, AuthDbContext>(_ =>
        {
            var connectionStr = configuration.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
            optionsBuilder.UseNpgsql(connectionStr);

            return new AuthDbContext(optionsBuilder.Options);
        });
    }
}