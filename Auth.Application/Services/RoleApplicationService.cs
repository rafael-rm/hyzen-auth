using Auth.Application.DTOs.Request;
using Auth.Application.DTOs.Response;
using Auth.Application.Interfaces;
using Auth.Application.Mappers.Interfaces;
using Auth.Domain.Entities;
using Auth.Domain.Exceptions;
using Auth.Domain.Interfaces.Services;

namespace Auth.Application.Services;

public class RoleApplicationService : IRoleApplicationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRoleService _roleService;
    private readonly IMapper<Role, RoleResponse> _mapperDto;
    
    public RoleApplicationService(IUnitOfWork unitOfWork, IRoleService roleService, IMapper<Role, RoleResponse> mapperDto)
    {
        _unitOfWork = unitOfWork;
        _roleService = roleService;
        _mapperDto = mapperDto;
    }

    public async Task<RoleResponse> CreateAsync(CreateRoleRequest createRoleRequest)
    {
        var role = new Role
        {
            Guid = Guid.NewGuid(),
            Name = createRoleRequest.Name,
            Description = createRoleRequest.Description,
        };
        
        await _roleService.CreateAsync(role);
        await _unitOfWork.CommitAsync();
        
        return _mapperDto.Map(role);
    }

    public async Task<RoleResponse> GetByGuidAsync(Guid roleId)
    {
        var role = await _roleService.GetByGuidAsync(roleId);

        if (role is null)
            throw new RoleNotFoundException(roleId);
        
        return _mapperDto.Map(role);
    }

    public async Task<RoleResponse> GetByNameAsync(string name)
    {
        var role = await _roleService.GetByNameAsync(name);
        
        if (role is null)
            throw new RoleNotFoundException(name);
        
        return _mapperDto.Map(role);
    }

    public async Task DeleteAsync(string name)
    {
        var role = await _roleService.GetByNameAsync(name);

        if (role is null)
            throw new RoleNotFoundException(name);

        await _roleService.DeleteAsync(role);
        await _unitOfWork.CommitAsync();
    }
}