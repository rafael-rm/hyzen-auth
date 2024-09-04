using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Auth.Core.Models;
using Hyzen.SDK.Authentication.DTO;
using Hyzen.SDK.Exception;
using Hyzen.SDK.SecretManager;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Core.Services;

public static class TokenService
{
    private static readonly string Secret = HyzenSecret.GetSecret("HYZEN-AUTH-TOKEN-SECRET");
    private static byte[] ByteSecret => Convert.FromBase64String(Secret);

    private static readonly TokenValidationParameters ValidationParameters = new()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(ByteSecret)
    };

    public static string GenerateToken(User request, out long issuedAt, int expirationHours = 6)
    {
        var issuanceDate = DateTime.UtcNow;
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
            IssuedAt = issuanceDate
        };
        
        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateJwtSecurityToken(descriptor);
        
        issuedAt = ((DateTimeOffset)issuanceDate).ToUnixTimeSeconds();
        return handler.WriteToken(token);
    }
    
    private static ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = (JwtSecurityToken)tokenHandler.ReadToken(token);
            
            if (jwt == null)
                return null;
            
            return tokenHandler.ValidateToken(token, ValidationParameters, out _);
        }
        catch
        {
            return null;
        }
    }
    
    public static async Task<AuthSubject> GetSubjectFromToken(string token)
    {
        var principal = GetPrincipalFromToken(token);
        if (principal == null)
            throw new HException("Invalid or expired token", ExceptionType.InvalidCredentials);
        
        var claims = principal.Claims.ToList();
        var subjectId = Guid.Parse(claims.First(s => s.Type == ClaimTypes.PrimarySid).Value);
        var issuedAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(claims.First(s => s.Type == "iat").Value)).UtcDateTime;
        
        var user = await User.GetAsync(subjectId);
        var roles = user.Roles.Select(s => s.Role.Name).ToList();
        var groups = user.Groups.Select(s => s.Group.Name).ToList();
        
        if (user.LastLoginAt > issuedAt)
            throw new HException("Invalid or expired token", ExceptionType.InvalidCredentials);
        
        return new AuthSubject
        {
            Guid = subjectId,
            Name = claims.First(s => s.Type == ClaimTypes.GivenName).Value,
            Email = claims.First(s => s.Type == ClaimTypes.Email).Value,
            Groups = groups,
            Roles = roles
        };
    }
}