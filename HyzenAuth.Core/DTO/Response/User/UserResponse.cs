using HyzenAuth.Core.DTO.Response.Role;

namespace HyzenAuth.Core.DTO.Response.User;

public record UserResponse
{
    public Guid Guid { get; set; } 
    public string Name { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<RoleResponse> Roles { get; set; } 
    
    public static UserResponse FromUser(Models.User user)
    {
        return new UserResponse
        {
            Guid = user.Guid,
            Name = user.Name,
            Email = user.Email,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            Roles = user.Roles.Select(s => RoleResponse.FromRole(s.Role)).ToList()
        };
    }
}