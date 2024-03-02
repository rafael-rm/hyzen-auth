using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
    
    public static async Task<Role> GetAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        
        return await AuthContext.Get().RolesSet.FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());
    }
    
    public void Delete()
    {
        AuthContext.Get().RolesSet.Remove(this);
    }
    
    public static async Task<Role> CreateAsync(string  name)
    {
        var role = new Role
        {
            Guid = Guid.NewGuid(),
            Name = name
        };

        await AuthContext.Get().RolesSet.AddAsync(role);

        return role;
    }
    
    public void Update(string name)
    {
        Name = name;
    }
}