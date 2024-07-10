namespace Auth.Core.DTO.Response.Auth;

public class VerifyResponse
{
    public Guid Guid { get; set; } 
    public string Name { get; set; }
    public string Email { get; set; }
    public List<string> Groups { get; set; }
    public List<string> Roles { get; set; } 
}