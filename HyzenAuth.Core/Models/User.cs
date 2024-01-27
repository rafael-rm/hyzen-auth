using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HyzenAuth.Core.DTO.Request;
using HyzenAuth.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HyzenAuth.Core.Models;

[Table("users")]
public class User
{
    private User() { }
    
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id", TypeName = "INT")] 
    public int Id { get; set; }
    
    [Column("guid", TypeName = "CHAR(36)"), Required] 
    public Guid Guid { get; set; } 
    
    [Column("name", TypeName = "VARCHAR(255)"), MaxLength(255), Required] 
    public string Name { get; set; }
    
    [Column("email", TypeName = "VARCHAR(255)"), MaxLength(255), Required] 
    public string Email { get; set; }
    
    [Column("is_active", TypeName = "BIT"), Required] 
    public bool IsActive { get; set; } 
    
    [Column("password", TypeName = "VARCHAR(255)"), MaxLength(255), Required] 
    public string Password { get; set; }
    
    [Column("created_at", TypeName = "DATETIME"), DatabaseGenerated(DatabaseGeneratedOption.Computed)] 
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at", TypeName = "DATETIME"), DatabaseGenerated(DatabaseGeneratedOption.Computed)] 
    public DateTime UpdatedAt { get; set; }
    
    public static async Task<User> GetAsync(int id)
    {
        return await AuthContext.Get().UsersSet.FirstOrDefaultAsync(s => s.Id == id && s.IsActive);
    }
    
    public static async Task<User> GetAsync(LoginRequest request)
    {
        return await AuthContext.Get().UsersSet.FirstOrDefaultAsync(s => s.Email == request.Email && s.Password == request.Password && s.IsActive);
    }
}