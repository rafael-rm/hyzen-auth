using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Hyzen.SDK.Exception;
using HyzenAuth.Core.DTO.Response.Auth;
using HyzenAuth.Core.Models;
using Microsoft.IdentityModel.Tokens;

namespace HyzenAuth.Core.Services;

public static class TokenService
{
    private const string Secret = "6FSx1+1AOUEImFI7KTMCFxceC7P0ZyiekaKTKTkGQGM="; // TODO: Save to an environment variable / AWS
    private static byte[] ByteSecret => Convert.FromBase64String(Secret);

    public static readonly TokenValidationParameters ValidationParameters = new()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(ByteSecret)
    };

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
        
        request.Groups.ForEach(s =>
        {
            descriptor.Subject.AddClaim(new Claim(ClaimTypes.GroupSid, s.Group.Name));
        });
        
        request.Roles.ForEach(s =>
        {
            descriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, s.Role.Name));
        });
        
        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateJwtSecurityToken(descriptor);
        return handler.WriteToken(token);
    }
    
    public static void ValidateToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        try
        {
            handler.ValidateToken(token, ValidationParameters, out _);
        }
        catch
        {
            throw new HException("Invalid or expired token", ExceptionType.InvalidCredentials);
        }
    }
    
    public static ClaimsPrincipal GetPrincipalFromToken(string token)
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
    
    public static VerifyResponse GetSubjectFromToken(string token)
    {
        var principal = GetPrincipalFromToken(token);
        if (principal == null)
            throw new HException("Invalid or expired token", ExceptionType.InvalidCredentials);
        
        var claims = principal.Claims.ToList();
        
        return new VerifyResponse
        {
            Guid = Guid.Parse(claims.First(s => s.Type == ClaimTypes.PrimarySid).Value),
            Name = claims.First(s => s.Type == ClaimTypes.GivenName).Value,
            Email = claims.First(s => s.Type == ClaimTypes.Email).Value,
            Groups = claims.Where(s => s.Type == ClaimTypes.GroupSid).Select(s => s.Value).ToList(),
            Roles = claims.Where(s => s.Type == ClaimTypes.Role).Select(s => s.Value).ToList()
        };
    }
}