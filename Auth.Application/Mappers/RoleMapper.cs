using Auth.Application.DTOs.Response;
using Auth.Application.Mappers.Interfaces;
using Auth.Domain.Entities;

namespace Auth.Application.Mappers
{
    public class RoleMapper : IMapper<Role, RoleResponse>, IMapper<RoleResponse, Role>
    {
        public RoleResponse Map(Role source)
        {
            return new RoleResponse
            {
                Name = source.Name,
                Description = source.Description,
            };
        }

        public Role Map(RoleResponse source)
        {
            return new Role
            {
                Name = source.Name,
                Description = source.Description,
            };
        }

        public IEnumerable<RoleResponse> Map(IEnumerable<Role> source)
        {
            return source.Select(Map);
        }

        public IEnumerable<Role> Map(IEnumerable<RoleResponse> source)
        {
            return source.Select(Map);
        }
    }
}