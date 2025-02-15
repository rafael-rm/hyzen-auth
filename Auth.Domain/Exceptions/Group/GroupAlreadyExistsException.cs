namespace Auth.Domain.Exceptions.Group;

public class GroupAlreadyExistsException(string key) : Exception($"Group with key {key} already exists.")
{
}