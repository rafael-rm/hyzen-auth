namespace HyzenAuth.Core.DTO.Response.Role;

public record RoleResponse
{
    public Guid Guid { get; set; } 
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public static RoleResponse FromUser(Models.Role role)
    {
        return new RoleResponse()
        {
            Guid = role.Guid,
            Name = role.Name,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt
        };
    }
}