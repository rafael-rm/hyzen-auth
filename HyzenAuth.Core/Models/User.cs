using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HyzenAuth.Core.DTO.Request.User;
using HyzenAuth.Core.Infrastructure;
using HyzenAuth.Core.Services;
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
    
    [InverseProperty("User")] 
    public List<UserRole> Roles { get; set; }
    
    public static async Task<User> GetAsync(string term)
    {
        var context =  AuthContext.Get().UsersSet.Include(s => s.Roles).ThenInclude(s => s.Role).AsQueryable();
        
        if (string.IsNullOrEmpty(term))
            return null;
            
        if (int.TryParse(term, out var id))
            return await context.FirstOrDefaultAsync(s => s.Id == id);
            
        if (Guid.TryParse(term, out var guid))
            return await context.FirstOrDefaultAsync(s => s.Guid == guid);

        return null;
    }
    
    public void Delete()
    {
        AuthContext.Get().UsersSet.Remove(this);
    }
    
    public static async Task<List<User>> SearchAsync(int? id = null, Guid? guid = null, string email = null, string password = null, bool? isActive = null)
    {
        var queryable =  AuthContext.Get().UsersSet.Include(s => s.Roles).ThenInclude(s => s.Role).AsQueryable();

        if (id is not null)
            queryable = queryable.Where(s => s.Id == id);

        if (guid is not null)
            queryable = queryable.Where(s => s.Guid == guid);

        if (!string.IsNullOrEmpty(email))
            queryable = queryable.Where(s => s.Email == email);

        if (!string.IsNullOrEmpty(password))
            queryable = queryable.Where(s => s.Password == password);

        if (isActive is not null)
            queryable = queryable.Where(s => s.IsActive == isActive);

        return await queryable.ToListAsync();
    }

    public static async Task<User> CreateAsync(CreateUserRequest request, bool isActive = true)
    {
        var user = new User
        {
            Guid = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            Password = PasswordHelper.HashPassword(request.Password),
            IsActive = isActive,
        };

        await AuthContext.Get().UsersSet.AddAsync(user);

        return user;
    }
    
    public void Update(UpdateUserRequest request)
    {
        Name = request.Name;
        Email = request.Email;
        Password = PasswordHelper.HashPassword(request.Password);
        IsActive = request.IsActive;
    }
    
    public async Task LoadRoles()
    {
        await AuthContext.Get().Entry(this)
            .Collection(s => s.Roles)
            .Query()
            .Include(x => x.Role)
            .LoadAsync();
    }

    public async Task<bool> HasRole(string role)
    {
        if (Roles is null)
            await LoadRoles();

        return Roles!.Any(s => s.Role.Name == role);
    }
}