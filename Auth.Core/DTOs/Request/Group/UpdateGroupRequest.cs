using System.ComponentModel.DataAnnotations;

namespace Auth.Core.DTOs.Request.Group;

public class UpdateGroupRequest
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; }
}