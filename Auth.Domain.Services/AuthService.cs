using Auth.Domain.Exceptions.User;
using Auth.Domain.Interfaces.Repositories;
using Auth.Domain.Interfaces.Services;

namespace Auth.Domain.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IHashService _hashService;
    private readonly ITokenService _tokenService;
    
    public AuthService(IUserRepository userRepository, IHashService hashService, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _hashService = hashService;
        _tokenService = tokenService;
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user is null)
            throw new UserNotFoundException(email);

        if (!_hashService.Verify(password, user.Password))
            throw new InvalidPasswordException();

        return _tokenService.GenerateToken(user);
    }
}