using System.ComponentModel.DataAnnotations;

namespace HyzenAuth.Core.DTO.Request.Auth;

public record LoginRequest
{
    [Required (ErrorMessage = "Email is required")]
    public string Email { get; set; }
    
    [Required (ErrorMessage = "Password is required")]
    public string Password { get; set; }
}