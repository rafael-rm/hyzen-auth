namespace HyzenAuth.Core.DTO.Request.Auth;

public record LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}