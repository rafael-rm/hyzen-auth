namespace Auth.Domain.Entities;

public class UserGroup
{
    public int UserId { get; set; }
    public User User { get; set; }

    public int GroupId { get; set; }
    public Group Group { get; set; }
    
    private UserGroup() { }
    
    public UserGroup(User user, Group group)
    {
        User = user;
        Group = group;
    }
}