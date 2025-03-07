using System.Security.Cryptography;
using Auth.Application.DTOs.Response.OpenId;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers;

[ApiController]
[Route(".well-known")]
public class OpenIdController(IConfiguration configuration) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("jwks")]
    public IActionResult GetJwks()
    {
        var publicKeyXml = configuration.GetValue<string>("Jwt:PublicKeyXml");
        
        using var rsa = RSA.Create();
        rsa.FromXmlString(publicKeyXml!);
        
        var jwk = new JwkResponse("ad3e3bff-049b-4950-8af5-be08ab21c5c1",
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
        var jwksUri = Url.Action("GetJwks", "OpenId", null, Request.Scheme);
        
        var issuer = configuration.GetValue<string>("Jwt:Issuer");
        
        var openIdConfiguration = new OpenIdConfigurationResponse(issuer, jwksUri);
        
        return Ok(openIdConfiguration);
    }
}