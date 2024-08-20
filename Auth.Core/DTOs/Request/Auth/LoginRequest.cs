using System.ComponentModel.DataAnnotations;

namespace Auth.Core.DTOs.Request.Auth;

public record LoginRequest
{
    [Required (ErrorMessage = "Email is required")]
    public string Email { get; set; }
    
    [Required (ErrorMessage = "Password is required")]
    public string Password { get; set; }
}