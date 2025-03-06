namespace Auth.Application.Interfaces.Infrastructure;

public interface IHashService
{
    string Hash(string password);
    bool Verify(string password, string storedHashWithSalt);
}