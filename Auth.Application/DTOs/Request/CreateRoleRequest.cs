using System.ComponentModel.DataAnnotations;

namespace Auth.Application.DTOs.Request;

public class CreateRoleRequest
{
    [Required(ErrorMessage = "Key is required.")]
    public string Key { get; set; }
    
    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Description is required.")]
    public string Description { get; set; }
}