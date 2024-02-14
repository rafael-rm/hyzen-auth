using System.Security.Cryptography;
using System.Text;

namespace HyzenAuth.Core.Services;

public class PasswordHelper
{
    private static readonly string Salt = "6FSx1+1AOUEImFI7KTMCFxceC7P0ZyiekaKTKTkGQGM="; // TODO: Save to an environment variable / AWS

    public static string HashPassword(string password)
    {
        var passwordAndSaltBytes = Concatenate(password, Convert.FromBase64String(Salt));
        return ComputeHash(passwordAndSaltBytes);
    }
        
    public static bool Verify(string password, string passwordHash)
    {
        var passwordAndSaltBytes = Concatenate(password, Convert.FromBase64String(Salt));
        var hashAttempt = ComputeHash(passwordAndSaltBytes);
        return passwordHash == hashAttempt;
    }

    private static string ComputeHash(byte[] bytes)
    {
        return Convert.ToBase64String(SHA256.HashData(bytes));
    }
        
    private static byte[] Concatenate(string password, byte[] saltBytes)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        return passwordBytes.Concat(saltBytes).ToArray();
    }
}