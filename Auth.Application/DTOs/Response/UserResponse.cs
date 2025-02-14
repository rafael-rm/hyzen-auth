using Auth.Domain.Entities;

namespace Auth.Application.DTOs.Response;

public class UserResponse
{
    public Guid Guid { get;  set; }
    public string Name { get;  set; }
    public string Email { get;  set; }
    public DateTime? LastLoginAt { get;  set; }
    public DateTime CreatedAt { get;  set; }
    public List<string> Roles { get; set; }
    
    public static UserResponse FromEntity(User user)
    {
        return new UserResponse
        {
            Guid = user.Guid,
            Name = user.Name,
            Email = user.Email,
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        };
    }
}