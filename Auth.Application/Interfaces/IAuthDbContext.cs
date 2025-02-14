using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.Interfaces;

public interface IAuthDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupRole> GroupRoles { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}