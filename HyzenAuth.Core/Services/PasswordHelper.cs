using System.Security.Cryptography;
using System.Text;

namespace HyzenAuth.Core.Services;

public static class PasswordHelper
{
    private const int SaltSize = 32;

    public static string Hash(string password)
    {
        var saltBytes = GenerateSalt(SaltSize);
        var passwordAndSaltBytes = Concatenate(password, saltBytes);
        var hash = ComputeHash(passwordAndSaltBytes);
        
        var hashWithSalt = new byte[saltBytes.Length + hash.Length];
        Buffer.BlockCopy(saltBytes, 0, hashWithSalt, 0, saltBytes.Length);
        Buffer.BlockCopy(hash, 0, hashWithSalt, saltBytes.Length, hash.Length);
        
        return Convert.ToBase64String(hashWithSalt);
    }
        
    public static bool Verify(string password, string storedHashWithSalt)
    {
        var hashWithSaltBytes = Convert.FromBase64String(storedHashWithSalt);
        
        var saltBytes = new byte[SaltSize];
        Buffer.BlockCopy(hashWithSaltBytes, 0, saltBytes, 0, SaltSize);
        
        var hashBytes = new byte[hashWithSaltBytes.Length - SaltSize];
        Buffer.BlockCopy(hashWithSaltBytes, SaltSize, hashBytes, 0, hashBytes.Length);
        
        var passwordAndSaltBytes = Concatenate(password, saltBytes);
        var hashAttempt = ComputeHash(passwordAndSaltBytes);
        return hashAttempt.SequenceEqual(hashBytes);
    }

    private static byte[] ComputeHash(byte[] bytes)
    {
        return SHA256.HashData(bytes);
    }
        
    private static byte[] Concatenate(string password, byte[] saltBytes)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        return passwordBytes.Concat(saltBytes).ToArray();
    }

    private static byte[] GenerateSalt(int size)
    {
        var saltBytes = new byte[size];
        RandomNumberGenerator.Fill(saltBytes);
        return saltBytes;
    }
}