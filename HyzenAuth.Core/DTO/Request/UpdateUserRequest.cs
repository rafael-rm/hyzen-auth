namespace HyzenAuth.Core.DTO.Request;

public class UpdateUserRequest
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public bool IsActive { get; set; }
}