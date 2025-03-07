using System.Text.Json.Serialization;

namespace Auth.Application.DTOs.Response.OpenId;

public class JwksResponse(JwkResponse[] keys)
{
    [JsonPropertyName("keys")]
    public JwkResponse[] Keys { get; } = keys;
}

public class JwkResponse(string kid, string n, string e)
{
    [JsonPropertyName("kty")]
    public string Kty { get; } = "RSA";

    [JsonPropertyName("use")]
    public string Use { get; } = "sig";

    [JsonPropertyName("kid")]
    public string Kid { get; } = kid;

    [JsonPropertyName("n")]
    public string N { get; } = n;

    [JsonPropertyName("e")]
    public string E { get; } = e;
}