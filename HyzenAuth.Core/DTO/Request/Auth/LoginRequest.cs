using System.ComponentModel.DataAnnotations;

namespace HyzenAuth.Core.DTO.Request.Auth;

public record LoginRequest
{
    [Required]
    public string Email { get; set; }
    
    [Required]
    public string Password { get; set; }
}