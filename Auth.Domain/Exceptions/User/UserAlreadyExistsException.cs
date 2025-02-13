namespace Auth.Domain.Exceptions.User;

public class UserAlreadyExistsException(string email) : Exception($"User with email {email} already exists.");