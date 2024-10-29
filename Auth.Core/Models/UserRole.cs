using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Auth.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Auth.Core.Models;

[Table("users_roles")]
public class UserRole
{
    private UserRole() { }
    
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id", TypeName = "INT")] 
    public int Id { get; set; }
    
    [Column("user_id", TypeName = "INT"), ForeignKey("User")] public int UserId { get; set; } 
    public User User { get; set; } 
    
    [Column("role_id", TypeName = "INT"), ForeignKey("Role")] public int RoleId { get; set; } 
    public Role Role { get; set; } 
    
    [Column("assigned_at", TypeName = "DATETIME"), DatabaseGenerated(DatabaseGeneratedOption.Computed)] 
    public DateTime AssignedAt { get; set; }
    
    public static async Task<UserRole> GetAsync(int userId, int roleId)
    {
        return await AuthContext.Get().UsersRolesSet
            .Include(s => s.User)
            .Include(s => s.Role)
            .FirstOrDefaultAsync(s => s.UserId == userId && s.RoleId == roleId);
    }
    
    public static async Task<List<UserRole>> GetAsyncFromUser(int userId)
    {
        return await AuthContext.Get().UsersRolesSet
            .Where(s => s.UserId == userId)
            .ToListAsync();
    }
    
    public static async Task Add(int userId, int roleId)
    {
        var userRole = new UserRole { UserId = userId, RoleId = roleId };
        
        await AuthContext.Get().UsersRolesSet.AddAsync(userRole);
    }
    
    public async Task<bool> DeleteAsync(int? excludedGroupId = null)
    {
        var userGroups = await UserGroup.ListAsyncFromUser(UserId);
        
        foreach (var userGroup in userGroups)
        {
            var groupRoles = await GroupRole.GetAsyncFromGroup(userGroup.GroupId);
            if (groupRoles.Any(gr => gr.RoleId == RoleId && gr.GroupId != excludedGroupId )) // Check if the user is associated with a group that contains this permission
                return false;
        }
        
        AuthContext.Get().UsersRolesSet.Remove(this);
        return true;
    }
    
    public static async Task<List<UserRole>> GetAsyncFromGroup(int groupId)
    {
        return await AuthContext.Get().UsersRolesSet
            .Include(s => s.User)
            .Include(s => s.Role)
            .Where(s => s.User.Groups.Any(g => g.GroupId == groupId))
            .ToListAsync();
    }
    
    public static async Task<List<UserRole>> GetAsyncFromRole(int roleId)
    {
        return await AuthContext.Get().UsersRolesSet
            .Include(s => s.User)
            .Include(s => s.Role)
            .Where(s => s.RoleId == roleId)
            .ToListAsync();
    }
}