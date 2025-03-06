using Auth.Application.Common;
using Auth.Application.DTOs.Request;
using Auth.Application.DTOs.Response;
using Auth.Application.Errors;
using Auth.Application.Interfaces.Application;
using Auth.Application.Interfaces.Infrastructure;
using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.Services;

public class RoleService(IAuthDbContext authDbContext) : IRoleService
{
    public async Task<Result> CreateAsync(CreateRoleRequest request)
    {
        if (await RoleExistsByKeyAsync(request.Key))
            return Result.Failure(RoleError.RoleAlreadyExists);

        var role = new Role(request.Key, request.Name, request.Description);
        
        await authDbContext.Roles.AddAsync(role);
        await authDbContext.SaveChangesAsync();
        
        return Result.Success(RoleResponse.FromEntity(role));
    }

    public async Task<Result> GetByKeyAsync(string key)
    {
        var role = await authDbContext.Roles.FirstOrDefaultAsync(s => s.Key == key);
        
        if (role is null)
            return Result.Failure(RoleError.RoleNotFound);
        
        return Result.Success(RoleResponse.FromEntity(role));
    }

    public async Task<Result> DeleteAsync(string key)
    {
        var role = await authDbContext.Roles.FirstOrDefaultAsync(s => s.Key == key);

        if (role is null)
            return Result.Failure(RoleError.RoleNotFound);

        authDbContext.Roles.Remove(role);
        await authDbContext.SaveChangesAsync();
        
        return Result.Success();
    }

    private async Task<bool> RoleExistsByKeyAsync(string key)
    {
        return await authDbContext.Roles.AnyAsync(s => s.Key == key);
    }
}
