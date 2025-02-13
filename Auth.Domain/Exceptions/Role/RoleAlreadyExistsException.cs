namespace Auth.Domain.Exceptions.Role;

public class RoleAlreadyExistsException(string name) : Exception($"Role with name {name} already exists.")
{
}