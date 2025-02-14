using Auth.Domain.Entities;

namespace Auth.Application.DTOs.Response;

public class RoleResponse
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public static RoleResponse FromEntity(Role role)
    {
        return new RoleResponse
        {
            Name = role.Name,
            Description = role.Description
        };
    }
}