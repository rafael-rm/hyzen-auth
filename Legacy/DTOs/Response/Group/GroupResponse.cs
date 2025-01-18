using Legacy.DTOs.Response.Role;

namespace Legacy.DTOs.Response.Group;

public class GroupResponse
{
    public Guid Guid { get; set; } 
    public string Name { get; set; }
    public string Description { get; set; }
    
    public static GroupResponse FromGroup(Models.Group group)
    {
        return new GroupResponse
        {
            Guid = group.Guid,
            Name = group.Name,
            Description = group.Description
        };
    }
}

public class GroupResponseWithRoles : GroupResponse
{
    public List<RoleResponse> Roles { get; set; }
    
    public new static GroupResponseWithRoles FromGroup(Models.Group group)
    {
        return new GroupResponseWithRoles
        {
            Guid = group.Guid,
            Name = group.Name,
            Description = group.Description,
            Roles = group.Roles?.Select(s => RoleResponse.FromRole(s.Role)).ToList()
        };
    }
    
    public static List<GroupResponseWithRoles> FromGroups(List<Models.Group> groups)
    {
        return groups.Select(FromGroup).ToList();
    }
}