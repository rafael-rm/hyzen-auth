namespace Auth.Domain.Core.Interfaces.Services;

public interface IHashService
{
    string Hash(string password);
    bool Verify(string password, string storedHashWithSalt);
}