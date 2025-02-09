using System.ComponentModel.DataAnnotations;

namespace Auth.Application.DTOs.Request;

public class CreateRoleRequest
{
    [Required]
    public required string Name { get; set; }
    [MaxLength(255)]
    public string? Description { get; set; }
}