using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HyzenAuth.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HyzenAuth.Core.Models;

[Table("groups")]
public class Group
{
    private Group() { }
    
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
    
    [InverseProperty("Group")]
    public List<GroupRole> Roles { get; set; }
    
    [InverseProperty("Group")]
    public List<UserGroup> UserGroups { get; set; }
    
    public static async Task<Group> GetAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        
        return await AuthContext.Get().GroupsSet
            .Include(s => s.Roles)
            .ThenInclude(s => s.Role)
            .FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());
    }
    
    public static async Task<Group> GetAsync(int id)
    {
        return await AuthContext.Get().GroupsSet
            .Include(s => s.Roles)
            .ThenInclude(s => s.Role)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
    
    public async Task DeleteAsync()
    {
        var groupRoles = await GroupRole.GetAsyncFromGroup(Id);
        
        foreach (var groupRole in groupRoles)
        {
            groupRole.Delete();
        }

        var userGroups = await UserGroup.GetAsyncFromGroup(Id);
        
        foreach (var userGroup in userGroups)
        {
            userGroup.Delete();
        }
        
        AuthContext.Get().GroupsSet.Remove(this);
    }
    
    public static async Task<Group> CreateAsync(string  name, List<string> roles)
    {
        var group = new Group
        {
            Guid = Guid.NewGuid(),
            Name = name
        };
        
        foreach (var roleName in roles)
        {
            var role = await Role.GetAsync(roleName);
            
            if (role == null)
                continue;
            
            _ = new GroupRole(role, group);
        }
        
        await AuthContext.Get().GroupsSet.AddAsync(group);
        
        return group;
    }
    
    public void Update(string name)
    {
        Name = name;
    }
    
    public async Task<bool> HasRoleAsync(string roleName)
    {
        return await AuthContext.Get().GroupsRolesSet.AnyAsync(s => s.Group == this && s.Role.Name == roleName);
    }
}