using Auth.Domain.Entities;

namespace Auth.Application.DTOs.Response;

public class GroupResponse
{
    public Guid Guid { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<string> Roles { get; set; } = [];
    
    public static GroupResponse FromEntity(Group group)
    {
        return new GroupResponse
        {
            Guid = group.Guid,
            Name = group.Name,
            Description = group.Description,
            CreatedAt = group.CreatedAt,
            UpdatedAt = group.UpdatedAt,
            Roles = group.GroupRoles.Select(gr => gr.Role.Name).ToList()
        };
    }
}