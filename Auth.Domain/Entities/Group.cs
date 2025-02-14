namespace Auth.Domain.Entities;

public class Group
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<UserGroup> UserGroups { get; set; }
    public ICollection<GroupRole> GroupRoles { get; set; }
    
    private Group() { }
    
    public Group(string name, string description)
    {
        Guid = Guid.NewGuid();
        Name = name;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        UserGroups = new List<UserGroup>();
        GroupRoles = new List<GroupRole>();
    }
}