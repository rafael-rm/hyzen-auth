using Auth.Domain.Core.Exceptions;
using Auth.Domain.Core.Interfaces.Repositories;
using Auth.Domain.Core.Interfaces.Services;

namespace Auth.Domain.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IHashService _hashService;
    private readonly ITokenService _tokenService;
    
    public AuthService(IUserRepository userRepository, IUnitOfWork unitOfWork, IHashService hashService, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
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