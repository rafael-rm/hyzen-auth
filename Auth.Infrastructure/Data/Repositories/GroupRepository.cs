using Auth.Domain.Entities;
using Auth.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Data.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly AuthDbContext _dbContext;

        public GroupRepository(AuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Group?> GetByIdAsync(int id)
        {
            return await _dbContext.Groups.FindAsync(id);
        }
        
        public async Task<Group?> GetByGuidAsync(Guid groupId)
        {
            return await _dbContext.Groups.FirstOrDefaultAsync(r => r.Guid == groupId);
        }

        public async Task<Group?> GetByNameAsync(string name)
        {
            return await _dbContext.Groups.FirstOrDefaultAsync(r => r.Name == name);
        }

        public async Task<IEnumerable<Group>> GetAllAsync()
        {
            return await _dbContext.Groups.ToListAsync();
        }

        public async Task AddAsync(Group role)
        {
            await _dbContext.Groups.AddAsync(role);
        }

        public void Delete(Group role)
        {
            _dbContext.Groups.Remove(role);
        }
    }
}