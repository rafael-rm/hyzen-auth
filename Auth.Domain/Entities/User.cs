namespace Auth.Domain.Entities;

public class User
{
    public int Id { get;  set; }
    public Guid Guid { get;  set; }
    public required string Name { get;  set; }
    public required string Email { get;  set; }
    public bool IsActive { get;  set; }
    public string Password { get;  set; } = string.Empty;
    public DateTime? LastLoginAt { get;  set; }
    public DateTime CreatedAt { get;  set; }
    public DateTime UpdatedAt { get;  set; }
    
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
}