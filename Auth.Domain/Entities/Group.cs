using Auth.Domain.Exceptions.Group;

namespace Auth.Domain.Entities;

public class Group
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public string Key { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<UserGroup> UserGroups { get; set; }
    public ICollection<GroupRole> GroupRoles { get; set; }
    
    private Group() { }
    
    public Group(string key, string name, string description)
    {
        Guid = Guid.NewGuid();
        Key = key;
        Name = name;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        UserGroups = new List<UserGroup>();
        GroupRoles = new List<GroupRole>();
    }
    
    public bool HasRole(string roleKey)
    {
        return GroupRoles.Any(gr => gr.Role.Key == roleKey);
    }
    
    public void AddRole(Role role)
    {
        if (HasRole(role.Key))
            throw new RoleAlreadyExistsInGroupException(role.Key);
        
        GroupRoles.Add(new GroupRole(this, role));
    }
    
    public void RemoveRole(Role role)
    {
        var groupRole = GroupRoles.FirstOrDefault(gr => gr.Role.Key == role.Key);
        if (groupRole is null)
            throw new RoleDoesNotExistsInGroupException(role.Key);
        
        GroupRoles.Remove(groupRole);
    }
    
    public void Update(string key, string name, string description)
    {
        Key = key;
        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }
}