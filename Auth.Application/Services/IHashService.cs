namespace Auth.Application.Services;

public interface IHashService
{
    string Hash(string password);
    bool Verify(string password, string storedHashWithSalt);
}