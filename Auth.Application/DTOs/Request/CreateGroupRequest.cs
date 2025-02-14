using System.ComponentModel.DataAnnotations;

namespace Auth.Application.DTOs.Request;

public class CreateGroupRequest
{
    [Required]
    public string Name { get; set; }
    [Required, MaxLength(255)]
    public string Description { get; set; }
}