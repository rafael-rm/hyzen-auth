namespace Auth.Domain.Exceptions;

public class UserAlreadyExistsException(string email) : Exception($"User with email {email} already exists.");