using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HyzenAuth.Core.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace HyzenAuth.Core.Models;

[Table("groups_roles")]
public class GroupRole
{
    private GroupRole() { }
    
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id", TypeName = "INT")] 
    public int Id { get; set; }
    
    [Column("group_id", TypeName = "INT"), ForeignKey("Group")] public int GroupId { get; set; }
    public Group Group { get; set; }
    
    [Column("role_id", TypeName = "INT"), ForeignKey("Role")] public int RoleId { get; set; } 
    public Role Role { get; set; } 
    
    [Column("assigned_at", TypeName = "DATETIME"), DatabaseGenerated(DatabaseGeneratedOption.Computed)] 
    public DateTime AssignedAt { get; set; }
    
    public GroupRole(Role role, Group group)
    {
        Role = role;
        Group = group;
        
        AuthContext.Get().GroupsRolesSet.Add(this);
    }
    
    public static async Task<GroupRole> GetAsync(int groupId, int roleId)
    {
        return await AuthContext.Get().GroupsRolesSet
            .FirstOrDefaultAsync(s => s.GroupId == groupId && s.RoleId == roleId);
    }
    
    public void Delete()
    {
        AuthContext.Get().GroupsRolesSet.Remove(this);
    }
    
    public static async Task Add(Group group, Role role)
    {
        var groupRole = new GroupRole { Group = group, Role = role };
        var userGroups = await UserGroup.GetAsyncFromGroup(group.Id);
    
        foreach (var userGroup in userGroups)
        {
            var userRoles = await UserRole.GetAsyncFromUser(userGroup.UserId);
            if (userRoles.All(ur => ur.RoleId != role.Id))
            {
                await UserRole.Add(userGroup.User, role);
            }
        }
    
        await AuthContext.Get().GroupsRolesSet.AddAsync(groupRole);
    }
    
    public static async Task Remove(int groupId, int roleId)
    {
        var groupRole = await GetAsync(groupId, roleId);
        var userRoles = await UserRole.GetAsyncFromGroup(groupId);
        
        foreach (var userRole in userRoles)
        {
            if (userRole.RoleId == roleId)
                _ = await UserRole.Remove(userRole.UserId, userRole.RoleId);
        }
        
        AuthContext.Get().GroupsRolesSet.Remove(groupRole);
    }
    
    public static async Task<List<GroupRole>> GetAsyncFromRole(int roleId)
    {
        return await AuthContext.Get().GroupsRolesSet
            .Include(s => s.Group)
            .Include(s => s.Role)
            .Where(s => s.RoleId == roleId)
            .ToListAsync();
    }
    
    public static async Task<List<GroupRole>> GetAsyncFromGroup(int groupId)
    {
        return await AuthContext.Get().GroupsRolesSet
            .Include(s => s.Group)
            .Include(s => s.Role)
            .Where(s => s.GroupId == groupId)
            .ToListAsync();
    }
}