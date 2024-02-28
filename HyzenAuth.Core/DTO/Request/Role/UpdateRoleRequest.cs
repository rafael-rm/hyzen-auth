namespace HyzenAuth.Core.DTO.Request.Role;

public record UpdateRoleRequest
{
    public string Id { get; set; }
    public string Name { get; set; }
}