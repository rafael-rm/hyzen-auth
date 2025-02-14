namespace Auth.Domain.Exceptions.User;

public class AuthenticationFailedException() : Exception("Invalid username or password.")
{
}