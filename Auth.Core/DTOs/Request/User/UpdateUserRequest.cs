using System.ComponentModel.DataAnnotations;

namespace Auth.Core.DTOs.Request.User;

public record UpdateUserRequest
{
    [Required (ErrorMessage = "Email is required")]
    public string Email { get; set; }
    
    [Required (ErrorMessage = "Name is required")]
    public string Name { get; set; }
    
    [Required (ErrorMessage = "Password is required")]
    public string Password { get; set; }
    
    [Required (ErrorMessage = "IsActive is required")]
    public bool IsActive { get; set; }
}