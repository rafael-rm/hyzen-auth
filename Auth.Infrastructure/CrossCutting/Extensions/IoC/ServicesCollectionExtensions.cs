using Auth.Domain.Core.Interfaces.Repositories;
using Auth.Domain.Core.Interfaces.Services;
using Auth.Domain.Services;
using Auth.Infrastructure.Data;
using Auth.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Infrastructure.CrossCutting.Extensions.IoC
{
    public static class ServicesCollectionExtensions
    {
        public static void AddAuthDbContext(this IServiceCollection services)
        {
            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseNpgsql("");
            });
        }
    
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
        }
    
        public static void AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
        }
    }
}