namespace Auth.Application.DTOs.Response;

public class UserResponse
{
    public Guid Guid { get;  set; }
    public string Name { get;  set; }
    public string Email { get;  set; }
    public DateTime? LastLoginAt { get;  set; }
    public DateTime CreatedAt { get;  set; }
}