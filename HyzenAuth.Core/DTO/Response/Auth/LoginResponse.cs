namespace HyzenAuth.Core.DTO.Response.Auth;

public record LoginResponse
{
    public string Token { get; set; }
}