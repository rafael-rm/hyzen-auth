﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Auth.Domain.Entities;
using Auth.Domain.Interfaces.Services;
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