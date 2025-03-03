using Auth.Application.Interfaces.ApplicationServices;
using Auth.Application.Interfaces.InfrastructureServices;
using Auth.Application.Services;
using Auth.Infrastructure.Data;
using Auth.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Auth.Infrastructure.CrossCutting.Extensions.IoC;

public static class ServicesCollectionExtensions
{
    public static void AddAuthDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuthDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            options.LogTo(Console.WriteLine, LogLevel.Information);
        });
        
        services.AddScoped<IAuthDbContext>(provider => provider.GetRequiredService<AuthDbContext>());
    }
        
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRoleService, RoleService>();
    }
        
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IHashService, Pbkdf2HashService>();
        services.AddSingleton<ITokenService>(_ => new JwtService(configuration["Token:Secret"] ?? string.Empty));
    }
}