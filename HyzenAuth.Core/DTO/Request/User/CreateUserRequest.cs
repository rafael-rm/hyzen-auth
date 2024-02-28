namespace HyzenAuth.Core.DTO.Request.User;

public record CreateUserRequest
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
}