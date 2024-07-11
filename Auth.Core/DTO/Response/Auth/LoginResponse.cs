using Auth.Core.Services;

namespace Auth.Core.DTO.Response.Auth;

public class LoginResponse : VerifyResponse
{
    public string Token { get; set; }
    
    public LoginResponse(string token)
    {
        var subject = TokenService.GetSubjectFromToken(token);
        
        Guid = subject.Guid;
        Name = subject.Name;
        Email = subject.Email;
        Groups = subject.Groups;
        Roles = subject.Roles;
        Token = token;
    }
}