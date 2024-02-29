using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HyzenAuth.Core.Infrastructure;
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
    
    public static async Task<UserRole> GetAsync(int id)
    {
        return await AuthContext.Get().UsersRolesSet
            .Include(s => s.User)
            .Include(s => s.Role)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
    
    public void Delete()
    {
        AuthContext.Get().UsersRolesSet.Remove(this);
    }
}