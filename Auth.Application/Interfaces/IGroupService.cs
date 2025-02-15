using Auth.Application.DTOs.Request;
using Auth.Application.DTOs.Response;

namespace Auth.Application.Interfaces;

public interface IGroupService
{
    Task<GroupResponse> CreateAsync(CreateGroupRequest request);
    Task<GroupResponse> GetByGuidAsync(Guid groupId);
    Task<GroupResponse> GetByKeyAsync(string key);
    Task DeleteAsync(string key);
    Task<GroupResponse> UpdateAsync(string key, UpdateGroupRequest request);
}