using System.ComponentModel.DataAnnotations;

namespace HyzenAuth.Core.DTO.Request.Group;

public class CreateGroupRequest
{
    [Required]
    public string Name { get; set; }
    
    public List<string> Roles { get; set; }
}