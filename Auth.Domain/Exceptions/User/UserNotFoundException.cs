namespace Auth.Domain.Exceptions.User;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(string email) : base($"User with email {email} not found.")
    {
    }
    
    public UserNotFoundException(Guid guid) : base($"User with guid {guid} not found.")
    {
    }
}