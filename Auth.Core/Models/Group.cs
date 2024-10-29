﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Auth.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Auth.Core.Models;

[Table("groups")]
public class Group
{
    private Group() { }
    
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id", TypeName = "INT")] 
    public int Id { get; set; }
    
    [Column("guid", TypeName = "CHAR(36)"), Required] 
    public Guid Guid { get; set; } 
    
    [Column("name", TypeName = "VARCHAR(64)"), MaxLength(64), Required] 
    public string Name { get; set; }
    
    [Column("description", TypeName = "VARCHAR(255)"), MaxLength(255)]
    public string Description { get; set; }
    
    [Column("created_at", TypeName = "DATETIME"), DatabaseGenerated(DatabaseGeneratedOption.Computed)] 
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at", TypeName = "DATETIME"), DatabaseGenerated(DatabaseGeneratedOption.Computed)] 
    public DateTime UpdatedAt { get; set; }
    
    [InverseProperty("Group")]
    public List<GroupRole> Roles { get; set; }
    
    public static async Task<Group> GetAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        
        return await AuthContext.Get().GroupsSet
            .Include(s => s.Roles)
            .ThenInclude(s => s.Role)
            .FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());
    }
    
    public static async Task<List<Group>> ListAsync()
    {
        return await AuthContext.Get().GroupsSet
            .Include(s => s.Roles)
            .ThenInclude(s => s.Role)
            .ToListAsync();
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
            await groupRole.DeleteAsync();
        }

        var userGroups = await UserGroup.GetAsyncFromGroup(Id);
        foreach (var userGroup in userGroups)
        {
            await userGroup.DeleteAsync();
        }
        
        AuthContext.Get().GroupsSet.Remove(this);
    }
    
    public static async Task<Group> CreateAsync(string name, string description, List<Role> roles)
    {
        var group = new Group
        {
            Guid = Guid.NewGuid(),
            Name = name,
            Description = description
        };
        
        foreach (var role in roles)
        {
            _ = new GroupRole(role, group);
        }
        
        await AuthContext.Get().GroupsSet.AddAsync(group);
        
        return group;
    }
    
    public static async Task<List<Group>> SearchAsync(int? id = null, Guid? guid = null, List<string> names = null)
    {
        var queryable =  AuthContext.Get().GroupsSet
            .Include(s => s.Roles)
            .ThenInclude(s => s.Role)
            .AsQueryable();
        
        if (id != null)
            queryable = queryable.Where(s => s.Id == id);
        
        if (guid != null)
            queryable = queryable.Where(s => s.Guid == guid);
        
        if (names != null)
            queryable = queryable.Where(s => names.Contains(s.Name));
        
        return await queryable.ToListAsync();
    }
    
    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
    }
    
    public async Task<bool> HasRoleAsync(string roleName)
    {
        return await AuthContext.Get().GroupsRolesSet.AnyAsync(s => s.Group == this && s.Role.Name == roleName);
    }
}