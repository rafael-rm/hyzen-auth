namespace Auth.Domain.Entities;

public class User
{
    public int Id { get;  set; }
    public Guid Guid { get;  set; }
    public required string Name { get;  set; }
    public required string Email { get;  set; }
    public bool IsActive { get;  set; }
    public string Password { get;  set; } = null!;
    public DateTime? LastLoginAt { get;  set; }
    public DateTime CreatedAt { get;  set; }
    public DateTime UpdatedAt { get;  set; }
}