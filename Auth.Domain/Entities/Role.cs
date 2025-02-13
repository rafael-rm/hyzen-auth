namespace Auth.Domain.Entities;

public class Role
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<GroupRole> GroupRoles { get; set; } = new List<GroupRole>();
}