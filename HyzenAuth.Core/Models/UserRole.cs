using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HyzenAuth.Core.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace HyzenAuth.Core.Models;

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
    
    public static async Task<UserRole> GetAsync(User user, Role role)
    {
        return await AuthContext.Get().UsersRolesSet
            .Include(s => s.User)
            .Include(s => s.Role)
            .FirstOrDefaultAsync(s => s.User == user && s.Role == role);
    }
    
    public void Delete()
    {
        AuthContext.Get().UsersRolesSet.Remove(this);
    }
    
    public static async Task Add(User user, Role role)
    {
        var userRole = new UserRole { User = user, Role = role };
        
        await AuthContext.Get().UsersRolesSet.AddAsync(userRole);
    }
    
    public static async Task Remove(User user, Role role)
    {
        var userRole = await GetAsync(user, role);
        
        AuthContext.Get().UsersRolesSet.Remove(userRole);
    }
}