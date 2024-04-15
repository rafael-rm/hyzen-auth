namespace HyzenAuth.Core.DTO.Request.Group;

public class CreateGroupRequest
{
    public string Name { get; set; }
    public List<string> Roles { get; set; }
}