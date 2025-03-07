namespace Auth.Domain.Entities;

public class UserRole
{
    public int UserId { get; set; }
    public User User { get; set; }

    public int RoleId { get; set; }
    public Role Role { get; set; }
    
    public DateTime AssignedAt { get; set; }
    
    private UserRole() { }
    
    public UserRole(User user, Role role)
    {
        UserId = user.Id;
        User = user;
        RoleId = role.Id;
        Role = role;
        AssignedAt = DateTime.UtcNow;
    }
}