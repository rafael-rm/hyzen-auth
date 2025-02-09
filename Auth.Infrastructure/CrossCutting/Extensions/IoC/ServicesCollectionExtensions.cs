using Auth.Application.DTOs.Response;
using Auth.Application.Interfaces;
using Auth.Application.Mappers;
using Auth.Application.Mappers.Interfaces;
using Auth.Application.Services;
using Auth.Domain.Entities;
using Auth.Domain.Interfaces.Repositories;
using Auth.Domain.Interfaces.Services;
using Auth.Domain.Services;
using Auth.Infrastructure.Data;
using Auth.Infrastructure.Data.Repositories;
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
    }
    
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
    }
    
    public static void AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRoleService, RoleService>();
    }
        
    public static void AddMappers(this IServiceCollection services)
    {
        services.AddScoped<RoleMapper>();
        
        services.AddScoped<IMapper<User, UserResponse>, UserMapper>();
        services.AddScoped<IMapper<UserResponse, User>, UserMapper>();
        services.AddScoped<IMapper<Role, RoleResponse>, RoleMapper>();
        services.AddScoped<IMapper<RoleResponse, Role>, RoleMapper>();
    }
        
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserApplicationService, UserApplicationService>();
        services.AddScoped<IAuthApplicationService, AuthApplicationService>();
        services.AddScoped<IRoleApplicationService, RoleApplicationService>();
    }
        
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IHashService, Pbkdf2HashService>();
        services.AddSingleton<ITokenService>(_ => new JwtService(configuration["Token:Secret"] ?? string.Empty));
    }
}