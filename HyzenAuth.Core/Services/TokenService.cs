using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HyzenAuth.Core.Models;
using Microsoft.IdentityModel.Tokens;

namespace HyzenAuth.Core.Services;

public class TokenService
{
    public const string Secret = "6FSx1+1AOUEImFI7KTMCFxceC7P0ZyiekaKTKTkGQGM="; // TODO: Save to an environment variable / AWS
    public static byte[] ByteSecret => Convert.FromBase64String(Secret);
    
    public static string GenerateToken(User request, int expirationHours = 6)
    {
        var securityKey = new SymmetricSecurityKey(ByteSecret);
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.PrimarySid, request.Guid.ToString()),
                new Claim(ClaimTypes.GivenName, request.Name),
                new Claim(ClaimTypes.Email, request.Email),
            }),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature),
            Expires = DateTime.UtcNow.AddHours(expirationHours),
            IssuedAt = DateTime.UtcNow
        };
        
        request.UserRoles.ForEach(s =>
        {
            descriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, s.Role.Name));
        });
        
        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateJwtSecurityToken(descriptor);
        return handler.WriteToken(token);
    }
}