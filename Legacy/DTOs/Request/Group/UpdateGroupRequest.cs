using System.ComponentModel.DataAnnotations;

namespace Legacy.DTOs.Request.Group;

public class UpdateGroupRequest
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; }
}