using Auth.Application.DTOs.Response;
using Auth.Application.Mappers.Interfaces;
using Auth.Domain.Entities;

namespace Auth.Application.Mappers
{
    public class GroupMapper : IMapper<Group, GroupResponse>, IMapper<GroupResponse, Group>
    {
        public GroupResponse Map(Group source)
        {
            return new GroupResponse
            {
                Name = source.Name,
                Description = source.Description,
            };
        }

        public Group Map(GroupResponse source)
        {
            return new Group
            {
                Name = source.Name,
                Description = source.Description,
            };
        }

        public IEnumerable<GroupResponse> Map(IEnumerable<Group> source)
        {
            return source.Select(Map);
        }

        public IEnumerable<Group> Map(IEnumerable<GroupResponse> source)
        {
            return source.Select(Map);
        }
    }
}
