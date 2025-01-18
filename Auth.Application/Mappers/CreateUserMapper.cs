using Auth.Application.DTOs;
using Auth.Application.Mappers.Interfaces;
using Auth.Domain.Entities;

namespace Auth.Application.Mappers;

public class CreateUserMapper : IMapper<User, CreateUserDto>, IMapper<CreateUserDto, User>
{
    public CreateUserDto Map(User source)
    {
        return new CreateUserDto
        {
            Name = source.Name,
            Email = source.Email,
            Password = source.Password
        };
    }

    public User Map(CreateUserDto source)
    {
        return new User
        {
            Name = source.Name,
            Email = source.Email,
            Password = source.Password
        };
    }
    
    public IEnumerable<CreateUserDto> Map(IEnumerable<User> source)
    {
        return source.Select(Map);
    }
    
    public IEnumerable<User> Map(IEnumerable<CreateUserDto> source)
    {
        return source.Select(Map);
    }
}