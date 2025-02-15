namespace Auth.Domain.Exceptions.Group;

public class RoleDoesNotExistsInGroupException(string key) : Exception($"Role with key {key} does not exists in group.")
{
}