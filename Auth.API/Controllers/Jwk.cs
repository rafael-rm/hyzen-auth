using System.Security.Cryptography;
using Auth.Application.DTOs.Response.Jwk;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers;

[ApiController]
[Route(".well-known")]
public class Jwk : ControllerBase
{
    private readonly IConfiguration _configuration;
    
    public Jwk(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    [AllowAnonymous]
    [HttpGet("jwks")]
    public IActionResult GetJwks()
    {
        var publicKeyXml = _configuration.GetValue<string>("Jwt:PublicKeyXml");
        
        using var rsa = RSA.Create();
        rsa.FromXmlString(publicKeyXml!);
        
        var jwk = new JwkResponse(
            "v1",
            Base64UrlTextEncoder.Encode(rsa.ExportParameters(false).Modulus!),
            Base64UrlTextEncoder.Encode(rsa.ExportParameters(false).Exponent!)
        );

        var jwks = new JwksResponse([jwk]);
        
        return Ok(jwks);
    }
    
    [AllowAnonymous]
    [HttpGet("openid-configuration")]
    [ProducesResponseType(typeof(OpenIdConfigurationResponse), StatusCodes.Status200OK)]
    public IActionResult GetOpenIdConfiguration()
    {
        var jwksUri = Url.Action("GetJwks", "Jwk", null, Request.Scheme);
        
        var issuer = _configuration.GetValue<string>("Jwt:Issuer");
        
        var openIdConfiguration = new OpenIdConfigurationResponse(issuer, jwksUri);
        
        return Ok(openIdConfiguration);
    }
}