using System.Text.Json.Serialization;

namespace Auth.Application.DTOs.Response.Jwk;

public class JwksResponse
{
    [JsonPropertyName("keys")]
    public JwkResponse[] Keys { get; }

    public JwksResponse(JwkResponse[] keys)
    {
        Keys = keys;
    }
}

public class JwkResponse
{
    [JsonPropertyName("kty")]
    public string Kty { get; }

    [JsonPropertyName("use")]
    public string Use { get; }

    [JsonPropertyName("kid")]
    public string Kid { get; }

    [JsonPropertyName("n")]
    public string N { get; }

    [JsonPropertyName("e")]
    public string E { get; }

    public JwkResponse(string kid, string n, string e)
    {
        Kty = "RSA";
        Use = "sig";
        Kid = kid;
        N = n;
        E = e;
    }
}