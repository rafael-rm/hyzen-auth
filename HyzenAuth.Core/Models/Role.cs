using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HyzenAuth.Core.DTO.Request.Role;
using HyzenAuth.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HyzenAuth.Core.Models;

[Table("roles")]
public class Role
{
    private Role() { }
    
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id", TypeName = "INT")] 
    public int Id { get; set; }
    
    [Column("guid", TypeName = "CHAR(36)"), Required] 
    public Guid Guid { get; set; } 
    
    [Column("name", TypeName = "VARCHAR(255)"), MaxLength(64), Required] 
    public string Name { get; set; }
    
    [Column("created_at", TypeName = "DATETIME"), DatabaseGenerated(DatabaseGeneratedOption.Computed)] 
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at", TypeName = "DATETIME"), DatabaseGenerated(DatabaseGeneratedOption.Computed)] 
    public DateTime UpdatedAt { get; set; }
    
    public static async Task<Role> GetAsync(string term)
    {
        if (string.IsNullOrEmpty(term))
            return null;
            
        if (int.TryParse(term, out var id))
            return await AuthContext.Get().RolesSet.FirstOrDefaultAsync(s => s.Id == id);
        
        if (Guid.TryParse(term, out var guid))
            return await AuthContext.Get().RolesSet.FirstOrDefaultAsync(s => s.Guid == guid);
        
        return await AuthContext.Get().RolesSet.FirstOrDefaultAsync(s => s.Name == term);
    }
    
    public void Delete()
    {
        AuthContext.Get().RolesSet.Remove(this);
    }
    
    public static async Task<Role> CreateAsync(CreateRoleRequest request)
    {
        var role = new Role
        {
            Guid = Guid.NewGuid(),
            Name = request.Name
        };

        await AuthContext.Get().RolesSet.AddAsync(role);

        return role;
    }
    
    public void Update(UpdateRoleRequest request)
    {
        Name = request.Name;
    }
}