using HyzenAuth.Core.DTO.Response.Role;

namespace HyzenAuth.Core.DTO.Response.Group;

public class GroupResponse
{
    public Guid Guid { get; set; } 
    public string Name { get; set; }
    public List<RoleResponse> Roles { get; set; }
    
    public static GroupResponse FromGroup(Models.Group group)
    {
        return new GroupResponse
        {
            Guid = group.Guid,
            Name = group.Name,
            Roles = group.Roles?.Select(s => RoleResponse.FromRole(s.Role)).ToList()
        };
    }
}