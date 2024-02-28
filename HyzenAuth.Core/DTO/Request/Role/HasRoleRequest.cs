namespace HyzenAuth.Core.DTO.Request.Role;

public record HasRoleRequest
{
    public Guid UserGuid { get; set; }
    public string Role { get; set; }
}