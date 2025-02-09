using Auth.Domain.Entities;
using Auth.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;
    
    public UserRepository(AuthDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
       return await _context.Users
           .Include(s => s.UserRoles)
           .ThenInclude(s => s.Role)
           .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByGuidAsync(Guid userId)
    {
        return await _context.Users
            .Include(s => s.UserRoles)
            .ThenInclude(s => s.Role)
            .FirstOrDefaultAsync(u => u.Guid == userId);
        
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(s => s.UserRoles)
            .ThenInclude(s => s.Role)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public Task UpdateAsync(User user)
    {
        throw new NotImplementedException();
    }

    public void Delete(User user)
    {
        _context.Users.Remove(user);
    }
}