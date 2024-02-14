using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HyzenAuth.Core.Models;
using Microsoft.IdentityModel.Tokens;

namespace HyzenAuth.Core.Services;

public class TokenService
{
    private const string Secret = "6FSx1+1AOUEImFI7KTMCFxceC7P0ZyiekaKTKTkGQGM="; // TODO: Save to an environment variable / AWS
    private static byte[] ByteSecret => Convert.FromBase64String(Secret);
    
    public static string GenerateToken(User request)
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
            Expires = DateTime.UtcNow.AddHours(6),
            IssuedAt = DateTime.UtcNow
        };
        
        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateJwtSecurityToken(descriptor);
        return handler.WriteToken(token);
    }
}