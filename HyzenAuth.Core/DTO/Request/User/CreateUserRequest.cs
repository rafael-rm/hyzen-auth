using System.ComponentModel.DataAnnotations;

namespace HyzenAuth.Core.DTO.Request.User;

public record CreateUserRequest
{
    [Required]
    public string Email { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string Password { get; set; }
}