namespace Auth.Domain.Exceptions;

public class RoleAlreadyExistsException(string name) : Exception($"Role with name {name} already exists.")
{
}