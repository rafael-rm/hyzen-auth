using Auth.Application.DTOs.Request;
using Auth.Application.Mappers.Interfaces;
using Auth.Domain.Entities;

namespace Auth.Application.Mappers;

public class CreateUserMapper : IMapper<User, CreateUserRequest>, IMapper<CreateUserRequest, User>
{
    public CreateUserRequest Map(User source)
    {
        return new CreateUserRequest
        {
            Name = source.Name,
            Email = source.Email,
            Password = source.Password
        };
    }

    public User Map(CreateUserRequest source)
    {
        return new User
        {
            Name = source.Name,
            Email = source.Email,
            Password = source.Password
        };
    }
    
    public IEnumerable<CreateUserRequest> Map(IEnumerable<User> source)
    {
        return source.Select(Map);
    }
    
    public IEnumerable<User> Map(IEnumerable<CreateUserRequest> source)
    {
        return source.Select(Map);
    }
}