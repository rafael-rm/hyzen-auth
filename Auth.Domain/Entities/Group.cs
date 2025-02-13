namespace Auth.Domain.Entities;

public class Group
{
    public int Id { get; set; }
    public Guid Guid { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
    public ICollection<GroupRole> GroupRoles { get; set; } = new List<GroupRole>();
}