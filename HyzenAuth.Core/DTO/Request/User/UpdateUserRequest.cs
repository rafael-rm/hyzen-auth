namespace HyzenAuth.Core.DTO.Request.User;

public record UpdateUserRequest
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public bool IsActive { get; set; }
}