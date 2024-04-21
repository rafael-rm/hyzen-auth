using System.ComponentModel.DataAnnotations;

namespace HyzenAuth.Core.DTO.Request.User;

public record UpdateUserRequest
{
    [Required]
    public string Email { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string Password { get; set; }
    
    [Required]
    public bool IsActive { get; set; }
}