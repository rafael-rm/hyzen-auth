using System.Security.Cryptography;
using Auth.Application.DTOs.Response.Jwk;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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
    
    [HttpGet("jwks")]
    public IActionResult GetJwks()
    {
        var publicKeyXml = _configuration["Jwt:PublicKeyXml"];

        using var rsa = RSA.Create();
        rsa.FromXmlString(publicKeyXml!);
        
        var publicKey = new RsaSecurityKey(rsa);

        var jwk = new JwkResponse(
            publicKey.KeyId,
            Base64UrlTextEncoder.Encode(rsa.ExportParameters(false).Modulus!),
            Base64UrlTextEncoder.Encode(rsa.ExportParameters(false).Exponent!)
        );

        var jwks = new JwksResponse([jwk]);
        
        return Ok(jwks);
    }
    
    [HttpGet("openid-configuration")]
    [ProducesResponseType(typeof(OpenIdConfigurationResponse), StatusCodes.Status200OK)]
    public IActionResult GetOpenIdConfiguration()
    {
        var jwksUri = Url.Action("GetJwks", "Jwk", null, Request.Scheme);
        
        var openIdConfiguration = new OpenIdConfigurationResponse("https://localhost:5021", jwksUri);
        
        return Ok(openIdConfiguration);
    }
}