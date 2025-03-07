using System.Text.Json.Serialization;

namespace Auth.Application.DTOs.Response.OpenId;

public class OpenIdConfigurationResponse(string issuer, string jwksUri)
{
    [JsonPropertyName("issuer")]
    public string Issuer { get; } = issuer;

    [JsonPropertyName("jwks_uri")]
    public string JwksUri { get; } = jwksUri;

    [JsonPropertyName("response_types_supported")]
    public string[] ResponseTypesSupported { get; } = ["code"];

    [JsonPropertyName("subject_types_supported")]
    public string[] SubjectTypesSupported { get; } = ["public"];

    [JsonPropertyName("id_token_signing_alg_values_supported")]
    public string[] IdTokenSigningAlgValuesSupported { get; } = ["RS256"];
}