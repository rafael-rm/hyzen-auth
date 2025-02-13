namespace Auth.Domain.Exceptions.Group;

public class GroupNotFoundException : Exception
{
    public GroupNotFoundException(string name) : base($"Group with name {name} not found.")
    {
    }
    
    public GroupNotFoundException(Guid guid) : base($"Group with guid {guid} not found.")
    {
    }
}