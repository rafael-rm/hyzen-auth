using Auth.Domain.Core.Interfaces.Repositories;
using Auth.Domain.Entities;
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
       return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByGuidAsync(Guid userId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Guid == userId);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
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