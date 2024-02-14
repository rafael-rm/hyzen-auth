using HyzenAuth.Core.Models;

namespace HyzenAuth.Core.DTO.Response;

public class UserResponse
{
    public Guid Guid { get; set; } 
    public string Name { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
    
    public static UserResponse FromUser(User user)
    {
        return new UserResponse()
        {
            Guid = user.Guid,
            Name = user.Name,
            Email = user.Email,
            IsActive = user.IsActive
        };
    }
}