namespace Auth.Domain.Exceptions.Group;

public class RoleAlreadyExistsInGroupException(string key) : Exception($"Role with key {key} already exists in group")
{
}