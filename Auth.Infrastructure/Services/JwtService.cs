using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Auth.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Auth.Application.Interfaces.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace Auth.Infrastructure.Services;

public class JwtService : ITokenService
{
    private readonly string _publicKeyXml;
    private readonly string _privateKeyXml;
    private readonly string _issuer;

    public JwtService(IConfiguration configuration)
    {
        var publicKeyXml = configuration["Jwt:PublicKeyXml"];
        var privateKeyXml = configuration["Jwt:PrivateKeyXml"];
        var issuer = configuration["Jwt:Issuer"];
        
        if (string.IsNullOrWhiteSpace(publicKeyXml))
            throw new ArgumentException("Public key cannot be null or empty.", nameof(publicKeyXml));

        if (string.IsNullOrWhiteSpace(privateKeyXml))
            throw new ArgumentException("Private key cannot be null or empty.", nameof(privateKeyXml));
        
        if (string.IsNullOrWhiteSpace(issuer))
            throw new ArgumentException("Issuer cannot be null or empty.", nameof(issuer));

        _publicKeyXml = publicKeyXml;
        _privateKeyXml = privateKeyXml;
        _issuer = issuer;
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
            Audience = "all",
            Issuer = _issuer
        };
        
        descriptor.Subject.AddClaims(request.UserRoles.Select(ur => new Claim(ClaimTypes.Role, ur.Role.Name)));

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateJwtSecurityToken(descriptor);

        token.Header["kid"] = "ad3e3bff-049b-4950-8af5-be08ab21c5c1";

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