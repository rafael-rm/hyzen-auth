using Auth.Application.DTOs.Response;
using Auth.Application.Mappers.Interfaces;
using Auth.Domain.Entities;

namespace Auth.Application.Mappers;

public class UserMapper : IMapper<User, UserResponse>, IMapper<UserResponse, User>
{
    private readonly RoleMapper _roleMapper;
    
    public UserMapper(RoleMapper roleMapper)
    {
        _roleMapper = roleMapper;
    }
    
    public UserResponse Map(User source)
    {
        return new UserResponse
        {
            Guid = source.Guid,
            Name = source.Name,
            Email = source.Email,
            CreatedAt = source.CreatedAt,
            LastLoginAt = source.LastLoginAt,
            Roles = source.UserRoles.Select(ur => ur.Role.Name).ToList()
        };
    }

    public User Map(UserResponse source)
    {
        return new User
        {
            Guid = source.Guid,
            Name = source.Name,
            Email = source.Email,
            CreatedAt = source.CreatedAt,
            LastLoginAt = source.LastLoginAt,
        };
    }
    
    public IEnumerable<UserResponse> Map(IEnumerable<User> source)
    {
        return source.Select(Map);
    }
    
    public IEnumerable<User> Map(IEnumerable<UserResponse> source)
    {
        return source.Select(Map);
    }
}