using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Auth.Application.Interfaces.InfrastructureServices;
using Auth.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Auth.Infrastructure.Services;

public class JwtService : ITokenService
{
    private readonly string _publicKeyXml;
    private readonly string _privateKeyXml;

    public JwtService(string publicKeyXml, string privateKeyXml)
    {
        if (string.IsNullOrWhiteSpace(publicKeyXml))
            throw new ArgumentException("Public key cannot be null or empty.", nameof(publicKeyXml));

        if (string.IsNullOrWhiteSpace(privateKeyXml))
            throw new ArgumentException("Private key cannot be null or empty.", nameof(privateKeyXml));

        _publicKeyXml = publicKeyXml;
        _privateKeyXml = privateKeyXml;
    }

    private RSAParameters PublicKeyParameters => GetRsaParameters(_publicKeyXml);
    private RSAParameters PrivateKeyParameters => GetRsaParameters(_privateKeyXml);

    private RSAParameters GetRsaParameters(string keyXml)
    {
        using var rsa = RSA.Create();
        rsa.FromXmlString(keyXml);
        return rsa.ExportParameters(true);
    }

    private TokenValidationParameters ValidationParameters => new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new RsaSecurityKey(RSA.Create(PublicKeyParameters)),
        ClockSkew = TimeSpan.Zero
    };

    public string GenerateToken(User request)
    {
        var issuanceDate = DateTime.UtcNow;
        var securityKey = new RsaSecurityKey(RSA.Create(PrivateKeyParameters));
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.PrimarySid, request.Guid.ToString()),
                new Claim(ClaimTypes.GivenName, request.Name),
                new Claim(ClaimTypes.Email, request.Email)
            ]),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256Signature),
            Expires = issuanceDate.AddHours(6),
            IssuedAt = issuanceDate,
            Audience = "auth",
            Issuer = "http://localhost:5021"
        };
        
        descriptor.Subject.AddClaims(request.UserRoles.Select(ur => new Claim(ClaimTypes.Role, ur.Role.Name)));

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateJwtSecurityToken(descriptor);

        return handler.WriteToken(token);
    }

    public async Task<bool> VerifyAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        var handler = new JwtSecurityTokenHandler();

        try
        {
            var result = await handler.ValidateTokenAsync(token, ValidationParameters);
            return result.IsValid;
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