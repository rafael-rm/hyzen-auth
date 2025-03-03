namespace Auth.Domain.Entities;

public class Role
{
    public int Id { get; set; }
    public string Key { get; set; }
    public Guid Guid { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    
    private Role() { }
    
    public Role(string key, string name, string description)
    {
        Guid = Guid.NewGuid();
        Key = key;
        Name = name;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}