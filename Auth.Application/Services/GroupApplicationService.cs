using Auth.Application.DTOs.Request;
using Auth.Application.DTOs.Response;
using Auth.Application.Interfaces;
using Auth.Application.Mappers.Interfaces;
using Auth.Domain.Entities;
using Auth.Domain.Exceptions.Group;
using Auth.Domain.Interfaces.Services;

namespace Auth.Application.Services;

public class GroupApplicationService : IGroupApplicationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGroupService _groupService;
    private readonly IMapper<Group, GroupResponse> _mapperDto;
    
    public GroupApplicationService(IUnitOfWork unitOfWork, IGroupService groupService, IMapper<Group, GroupResponse> mapperDto)
    {
        _unitOfWork = unitOfWork;
        _groupService = groupService;
        _mapperDto = mapperDto;
    }

    public async Task<GroupResponse> CreateAsync(CreateGroupRequest createGroupRequest)
    {
        throw new NotImplementedException();
    }

    public async Task<GroupResponse> GetByGuidAsync(Guid groupId)
    {
        var group = await _groupService.GetByGuidAsync(groupId);

        if (group is null)
            throw new GroupNotFoundException(groupId);
        
        return _mapperDto.Map(group);
    }

    public async Task<GroupResponse> GetByNameAsync(string name)
    {
        var group = await _groupService.GetByNameAsync(name);
        
        if (group is null)
            throw new GroupNotFoundException(name);
        
        return _mapperDto.Map(group);
    }

    public async Task DeleteAsync(string name)
    {
        var group = await _groupService.GetByNameAsync(name);

        if (group is null)
            throw new GroupNotFoundException(name);

        await _groupService.DeleteAsync(group);
        await _unitOfWork.CommitAsync();
    }
}