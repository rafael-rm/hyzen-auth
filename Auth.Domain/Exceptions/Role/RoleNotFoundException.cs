namespace Auth.Domain.Exceptions.Role;

public class RoleNotFoundException : Exception
{
    public RoleNotFoundException(string name) : base($"Role with name {name} not found.")
    {
    }
    
    public RoleNotFoundException(Guid guid) : base($"Role with guid {guid} not found.")
    {
    }
}