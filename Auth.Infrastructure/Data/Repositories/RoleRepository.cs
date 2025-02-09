using Auth.Domain.Entities;
using Auth.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Data.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AuthDbContext _dbContext;

        public RoleRepository(AuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _dbContext.Roles.FindAsync(id);
        }
        
        public async Task<Role?> GetByGuidAsync(Guid roleId)
        {
            return await _dbContext.Roles.FirstOrDefaultAsync(r => r.Guid == roleId);
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            return await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == name);
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _dbContext.Roles.ToListAsync();
        }

        public async Task AddAsync(Role role)
        {
            await _dbContext.Roles.AddAsync(role);
        }

        public void Delete(Role role)
        {
            _dbContext.Roles.Remove(role);
        }
    }
}