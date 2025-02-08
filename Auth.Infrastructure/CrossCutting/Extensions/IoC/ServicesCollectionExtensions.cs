using Auth.Application.DTOs.Request;
using Auth.Application.DTOs.Response;
using Auth.Application.Interfaces;
using Auth.Application.Mappers;
using Auth.Application.Mappers.Interfaces;
using Auth.Application.Services;
using Auth.Domain.Core.Interfaces.Repositories;
using Auth.Domain.Core.Interfaces.Services;
using Auth.Domain.Entities;
using Auth.Domain.Services;
using Auth.Infrastructure.Data;
using Auth.Infrastructure.Data.Repositories;
using Auth.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Infrastructure.CrossCutting.Extensions.IoC;

public static class ServicesCollectionExtensions
{
    public static void AddAuthDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuthDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });
    }
    
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
    }
    
    public static void AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
    }
        
    public static void AddMappers(this IServiceCollection services)
    {
        services.AddScoped<IMapper<User, UserResponse>, UserMapper>();
        services.AddScoped<IMapper<UserResponse, User>, UserMapper>();
        services.AddScoped<IMapper<CreateUserRequest, User>, CreateUserMapper>();
    }
        
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserApplicationService, UserApplicationService>();
        services.AddScoped<IAuthApplicationService, AuthApplicationService>();
    }
        
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IHashService, Pbkdf2HashService>();
        services.AddSingleton<ITokenService>(_ => new JwtService(configuration["Token:Secret"] ?? string.Empty));
    }
}