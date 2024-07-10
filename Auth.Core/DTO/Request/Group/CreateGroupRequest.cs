using System.ComponentModel.DataAnnotations;

namespace Auth.Core.DTO.Request.Group;

public class CreateGroupRequest
{
    [Required (ErrorMessage = "Name is required")]
    public string Name { get; set; }
    
    [Required (ErrorMessage = "Roles are required")]
    public List<string> Roles { get; set; }
}