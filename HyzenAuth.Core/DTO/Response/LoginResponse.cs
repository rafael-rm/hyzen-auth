using HyzenAuth.Core.Models;

namespace HyzenAuth.Core.DTO.Response;

public class LoginResponse
{
    public Guid Guid { get; set; } 
    public string Name { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static LoginResponse FromUser(User user, string token)
    {
        return new LoginResponse()
        {
            Guid = user.Guid,
            Name = user.Name,
            Email = user.Email,
            Token = token,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.CreatedAt
        };
    }
}