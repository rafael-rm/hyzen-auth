using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.Interfaces.Infrastructure;

public interface IAuthDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}