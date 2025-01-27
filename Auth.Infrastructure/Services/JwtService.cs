using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Auth.Application.Services;
using Auth.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Infrastructure.Services;

public class JwtService : ITokenService
{
    private readonly string _secret;

    public JwtService(string secret)
    {
        if (string.IsNullOrWhiteSpace(secret))
            throw new ArgumentException("Secret cannot be null or empty.", nameof(secret));

        _secret = secret;
    }

    private byte[] ByteSecret => Convert.FromBase64String(_secret);

    private TokenValidationParameters ValidationParameters => new()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(ByteSecret),
        ClockSkew = TimeSpan.Zero
    };

    public string GenerateToken(User request)
    {
        var issuanceDate = DateTime.UtcNow;
        var securityKey = new SymmetricSecurityKey(ByteSecret);
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.PrimarySid, request.Guid.ToString()),
                new Claim(ClaimTypes.GivenName, request.Name),
                new Claim(ClaimTypes.Email, request.Email)
            ]),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature),
            Expires = issuanceDate.AddHours(6),
            IssuedAt = issuanceDate
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateJwtSecurityToken(descriptor);

        return handler.WriteToken(token);
    }

    public bool ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or empty.", nameof(token));

        var handler = new JwtSecurityTokenHandler();

        try
        {
            handler.ValidateToken(token, ValidationParameters, out _);
            return true;
        }
        catch (SecurityTokenException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }
}