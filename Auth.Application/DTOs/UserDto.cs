namespace Auth.Application.DTOs;

public class UserDto
{
    public Guid Guid { get;  set; }
    public string Name { get;  set; }
    public string Email { get;  set; }
    public DateTime? LastLoginAt { get;  set; }
    public DateTime CreatedAt { get;  set; }
}