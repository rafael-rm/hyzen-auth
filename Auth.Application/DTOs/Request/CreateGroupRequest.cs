using System.ComponentModel.DataAnnotations;

namespace Auth.Application.DTOs.Request;

public class CreateGroupRequest
{
    [Required]
    public required string Name { get; set; }
    [Required, MaxLength(255)]
    public required string Description { get; set; }
}