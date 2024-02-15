using HyzenAuth.Core.Models;

namespace HyzenAuth.Core.DTO.Response;

public record LoginResponse : UserResponse
{
    public string Token { get; set; }
}