using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Auth.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Auth.Core.Models;

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
    
    [Column("description", TypeName = "VARCHAR(255)"), MaxLength(255)]
    public string Description { get; set; }
    
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
    
    public static async Task<List<Role>> ListAsync()
    {
        return await AuthContext.Get().RolesSet.ToListAsync();
    }
    
    public static async Task<List<Role>> SearchAsync(int? id = null, Guid? guid = null, List<string> names = null)
    {
        var queryable =  AuthContext.Get().RolesSet.AsQueryable();
        
        if (id != null)
            queryable = queryable.Where(s => s.Id == id);
        
        if (guid != null)
            queryable = queryable.Where(s => s.Guid == guid);
        
        if (names != null)
            queryable = queryable.Where(s => names.Contains(s.Name));
        
        return await queryable.ToListAsync();
    }
    public async Task DeleteAsync()
    {
        var groupsRole = await GroupRole.GetAsyncFromRole(Id);
        
        foreach (var groupRole in groupsRole)
        {
            await groupRole.DeleteAsync();
        }
        
        var userRoles = await UserRole.GetAsyncFromRole(Id);
        foreach (var userRole in userRoles)
        {
            _ = await userRole.DeleteAsync();
        }
        
        AuthContext.Get().RolesSet.Remove(this);
    }
    
    public static async Task<Role> CreateAsync(string  name, string description)
    {
        var role = new Role
        {
            Guid = Guid.NewGuid(),
            Name = name,
            Description = description
        };

        await AuthContext.Get().RolesSet.AddAsync(role);

        return role;
    }
    
    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
    }
}