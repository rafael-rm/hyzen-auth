using System.Security.Cryptography;
using Auth.Domain.Core.Interfaces.Services;

namespace Auth.Infrastructure.Services;

public class HashService : IHashService
{
    private const int SaltSize = 64;
    private const int HashSize = 64;
    private const int Iterations = 256000;

    public string Hash(string password)
    {
        var saltBytes = GenerateSalt(SaltSize);
        var hashBytes = ComputeHash(password, saltBytes, Iterations, HashSize);

        var hashWithSalt = new byte[saltBytes.Length + hashBytes.Length];
        Buffer.BlockCopy(saltBytes, 0, hashWithSalt, 0, saltBytes.Length);
        Buffer.BlockCopy(hashBytes, 0, hashWithSalt, saltBytes.Length, hashBytes.Length);

        return Convert.ToBase64String(hashWithSalt);
    }

    public bool Verify(string password, string storedHashWithSalt)
    {
        var hashWithSaltBytes = Convert.FromBase64String(storedHashWithSalt);

        var saltBytes = new byte[SaltSize];
        Buffer.BlockCopy(hashWithSaltBytes, 0, saltBytes, 0, SaltSize);

        var hashBytes = new byte[hashWithSaltBytes.Length - SaltSize];
        Buffer.BlockCopy(hashWithSaltBytes, SaltSize, hashBytes, 0, hashBytes.Length);

        var hashAttempt = ComputeHash(password, saltBytes, Iterations, HashSize);

        return CryptographicOperations.FixedTimeEquals(hashAttempt, hashBytes);
    }

    private static byte[] ComputeHash(string password, byte[] saltBytes, int iterations, int hashSize)
    {
        using var rfc2898 = new Rfc2898DeriveBytes(password, saltBytes, iterations, HashAlgorithmName.SHA512);
        return rfc2898.GetBytes(hashSize);
    }

    private static byte[] GenerateSalt(int size)
    {
        var saltBytes = new byte[size];
        RandomNumberGenerator.Fill(saltBytes);
        return saltBytes;
    }
}
