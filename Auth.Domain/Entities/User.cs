namespace Auth.Domain.Entities;

public class User
{
    public int Id { get;  set; }
    public Guid Guid { get;  set; }
    public string Name { get;  set; }
    public string Email { get;  set; }
    public bool IsActive { get;  set; }
    public string Password { get;  set; }
    public DateTime? LastLoginAt { get;  set; }
    public DateTime CreatedAt { get;  set; }
    public DateTime UpdatedAt { get;  set; }
    
    public ICollection<UserRole> UserRoles { get; set; }
    
    private User() { }
    
    public User(string name, string email, string hashPassword)
    {
        Guid = Guid.NewGuid();
        Name = name;
        Email = email;
        Password = hashPassword;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        UserRoles = new List<UserRole>();
    }
}