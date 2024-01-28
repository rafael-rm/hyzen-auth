using HyzenAuth.Core.Models;

namespace HyzenAuth.Core.DTO.Response;

public class CreateUserResponse
{
    public Guid Guid { get; set; } 
    public string Name { get; set; }
    public string Email { get; set; }
    
    public static CreateUserResponse FromUser(User user)
    {
        return new CreateUserResponse()
        {
            Guid = user.Guid,
            Name = user.Name,
            Email = user.Email,
        };
    }
}