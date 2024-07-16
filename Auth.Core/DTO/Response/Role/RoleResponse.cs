namespace Auth.Core.DTO.Response.Role;

public record RoleResponse
{
    public Guid Guid { get; set; } 
    public string Name { get; set; }
    public string Description { get; set; }
    
    public static RoleResponse FromRole(Models.Role role)
    {
        return new RoleResponse()
        {
            Guid = role.Guid,
            Name = role.Name,
            Description = role.Description
        };
    }
    
    public static List<RoleResponse> FromRoles(List<Models.Role> roles)
    {
        return roles.Select(FromRole).ToList();
    }
}