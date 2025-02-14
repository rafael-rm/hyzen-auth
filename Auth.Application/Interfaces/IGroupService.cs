using Auth.Application.DTOs.Request;
using Auth.Application.DTOs.Response;

namespace Auth.Application.Interfaces;

public interface IGroupService
{
    Task<GroupResponse> CreateAsync(CreateGroupRequest createGroupRequest);
    Task<GroupResponse> GetByGuidAsync(Guid groupId);
    Task<GroupResponse> GetByNameAsync(string name);
    Task DeleteAsync(string name);
}