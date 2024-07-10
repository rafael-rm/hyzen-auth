using System.ComponentModel.DataAnnotations;

namespace Auth.Core.DTO.Request.User;

public record CreateUserRequest
{
    [Required (ErrorMessage = "Email is required")]
    public string Email { get; set; }
    
    [Required (ErrorMessage = "Name is required")]
    public string Name { get; set; }
    
    [Required (ErrorMessage = "Password is required")]
    public string Password { get; set; }
}