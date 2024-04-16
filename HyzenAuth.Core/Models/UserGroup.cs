using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HyzenAuth.Core.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace HyzenAuth.Core.Models;

    [Table("users_groups")]
    public class UserGroup
    {
        private UserGroup() { }
    
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id", TypeName = "INT")] 
        public int Id { get; set; }
    
        [Column("user_id", TypeName = "INT"), ForeignKey("User")] 
        public int UserId { get; set; }
        public User User { get; set; }
    
        [Column("group_id", TypeName = "INT"), ForeignKey("Group")] 
        public int GroupId { get; set; }
        public Group Group { get; set; }
    
        [Column("assigned_at", TypeName = "DATETIME"), DatabaseGenerated(DatabaseGeneratedOption.Computed)] 
        public DateTime AssignedAt { get; set; }
    
    public static async Task<UserGroup> GetAsync(int userId, int groupId)
    {
        return await AuthContext.Get().UsersGroupsSet
            .FirstOrDefaultAsync(s => s.UserId == userId && s.GroupId == groupId);
    }
    
    public static async Task AddAsync(User user, Group group)
    {
        var userGroup = new UserGroup { User = user, Group = group };
        var userRoles = await UserRole.GetAsyncFromUser(user.Id);
        
        foreach (var groupRole in group.Roles)
        {
            var userRole = userRoles.FirstOrDefault(ur => ur.RoleId == groupRole.RoleId);
            if (userRole != null)
                continue;
            
            await UserRole.Add(user.Id, groupRole.RoleId);
        }
        
        await AuthContext.Get().UsersGroupsSet.AddAsync(userGroup);
    }
    
    public static async Task<List<UserGroup>> GetAsyncFromUser(int userId)
    {
        return await AuthContext.Get().UsersGroupsSet
            .Include(s => s.Group)
            .ThenInclude(s => s.Roles)
            .Where(s => s.UserId == userId)
            .ToListAsync();
    }
    
    public static async Task DeleteAsync(int userId, int groupId)
    {
        var group = await Group.GetAsync(groupId);
        var userGroups = await GetAsyncFromUser(userId);
        
        foreach (var groupRole in group.Roles)
        {
            var otherGroupHasRole = userGroups.Any(ug => ug.GroupId != groupId && ug.Group.Roles.Any(gr => gr.RoleId == groupRole.RoleId));
            if (otherGroupHasRole)
                continue;
            
            var userRole = await UserRole.GetAsync(userId, groupRole.RoleId);
            
            if (userRole != null)
               _ = await UserRole.DeleteAsync(userId, groupRole.RoleId, groupId);
        }
        
        AuthContext.Get().UsersGroupsSet.Remove(userGroups.First(s => s.UserId == userId && s.GroupId == groupId));
           
    }
    
    public static async Task<List<UserGroup>> GetAsyncFromGroup(int groupId)
    {
        return await AuthContext.Get().UsersGroupsSet
            .Include(s => s.User)
            .Include(s => s.Group)
            .Where(s => s.GroupId == groupId)
            .ToListAsync();
    }
}