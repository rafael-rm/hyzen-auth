namespace Auth.Application.DTOs.Response;

public class UserResponse
{
    public Guid Guid { get;  set; }
    public required string Name { get;  set; }
    public required string Email { get;  set; }
    public DateTime? LastLoginAt { get;  set; }
    public DateTime CreatedAt { get;  set; }
}