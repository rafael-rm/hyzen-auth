namespace HyzenAuth.Core.DTO.Request;

public record LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}