namespace Auth.Application.DTOs.Request;

public class UpdateGroupRequest
{
    public string Name { get; set; }
    
    public string Key { get; set; }
    
    public string Description { get; set; }
    
    public List<string> Roles { get; set; } = [];
}