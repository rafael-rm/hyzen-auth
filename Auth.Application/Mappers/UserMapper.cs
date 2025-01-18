using Auth.Application.DTOs;
using Auth.Application.Mappers.Interfaces;
using Auth.Domain.Entities;

namespace Auth.Application.Mappers;

public class UserMapper : IMapper<User, UserDto>, IMapper<UserDto, User>
{
    public UserDto Map(User source)
    {
        return new UserDto
        {
            Guid = source.Guid,
            Name = source.Name,
            Email = source.Email,
            CreatedAt = source.CreatedAt,
            LastLoginAt = source.LastLoginAt
        };
    }

    public User Map(UserDto source)
    {
        return new User
        {
            Guid = source.Guid,
            Name = source.Name,
            Email = source.Email,
            CreatedAt = source.CreatedAt,
            LastLoginAt = source.LastLoginAt
        };
    }
    
    public IEnumerable<UserDto> Map(IEnumerable<User> source)
    {
        return source.Select(Map);
    }
    
    public IEnumerable<User> Map(IEnumerable<UserDto> source)
    {
        return source.Select(Map);
    }
}