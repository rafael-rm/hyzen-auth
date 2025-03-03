namespace Auth.Application.Interfaces.InfrastructureServices;

public interface IHashService
{
    string Hash(string password);
    bool Verify(string password, string storedHashWithSalt);
}