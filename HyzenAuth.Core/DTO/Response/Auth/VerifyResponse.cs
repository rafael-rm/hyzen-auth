using HyzenAuth.Core.DTO.Response.Group;
using HyzenAuth.Core.DTO.Response.Role;

namespace HyzenAuth.Core.DTO.Response.Auth;

public class VerifyResponse
{
    public Guid Guid { get; set; } 
    public string Name { get; set; }
    public string Email { get; set; }
    public List<string> Groups { get; set; }
    public List<string> Roles { get; set; } 
    
    public bool HasRole(string roleKey)
    {
        var domains = roleKey.Split(":");
        if (domains.Length > 0)
            return Roles.Contains($"{domains[0]}:*") || Roles.Contains(roleKey);

        return Roles.Contains(roleKey);
    }
        
    public bool HasGroup(string groupKey)
    {
        return Groups.Contains(groupKey);
    }
}