namespace Auth.Application.DTOs.Request;

public class CreateUserRequest
{
    public string Name { get;  set; }
    public string Email { get;  set; }
    public string Password { get;  set; }
}