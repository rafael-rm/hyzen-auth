namespace Auth.Domain.Exceptions.Group;

public class GroupAlreadyExistsException(string name) : Exception($"Group with name {name} already exists.")
{
}