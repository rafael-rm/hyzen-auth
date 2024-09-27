using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Auth.Core.DTOs.Request.User;
using Auth.Core.Infrastructure;
using Auth.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace Auth.Core.Models;

[Table("users")]
public class User
{
    private User()
    {
    }

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
    
    [Column("last_login_at", TypeName = "DATETIME")]
    public DateTime LastLoginAt { get; set; }

    [Column("created_at", TypeName = "DATETIME"), DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at", TypeName = "DATETIME"), DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedAt { get; set; }

    [InverseProperty("User")] public List<UserRole> Roles { get; set; }

    [InverseProperty("User")] public List<UserGroup> Groups { get; set; }

    public static async Task<User> GetAsync(Guid id)
    {
        return await AuthContext.Get().UsersSet
            .Include(s => s.Roles)
            .ThenInclude(s => s.Role)
            .Include(s => s.Groups)
            .ThenInclude(s => s.Group)
            .FirstOrDefaultAsync(s => s.Guid == id);
    }
    
    public static async Task<User> GetAsync(string email)
    {
        return await AuthContext.Get().UsersSet
            .Include(s => s.Roles)
            .ThenInclude(s => s.Role)
            .Include(s => s.Groups)
            .ThenInclude(s => s.Group)
            .FirstOrDefaultAsync(s => s.Email == email);
    }

    public void Delete()
    {
        throw new NotImplementedException();
    }

    public static async Task<List<User>> SearchAsync(int? id = null, Guid? guid = null, string email = null, string password = null, bool? isActive = null)
    {
        var queryable = AuthContext.Get().UsersSet
            .Include(s => s.Roles)
            .ThenInclude(s => s.Role)
            .AsQueryable();

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
            Password = HashService.Hash(request.Password),
            IsActive = isActive,
        };

        await AuthContext.Get().UsersSet.AddAsync(user);

        return user;
    }

    public void Update(UpdateUserRequest request)
    {
        Name = request.Name;
        Email = request.Email;
    }
    
    public void ChangePassword(string password)
    {
        Password = HashService.Hash(password);
    }
    
    public void RegisterLoginEvent(long lastLoginAt)
    {
        LastLoginAt = DateTimeOffset.FromUnixTimeSeconds(lastLoginAt).UtcDateTime;
    }
    
    public void RegisterRecoveryPasswordEvent()
    {
        // TODO: Implement
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

        return Roles!.Any(s => s.Role.Name.ToLower() == role.ToLower());
    }

    public async Task LoadUserGroups()
    {
        await AuthContext.Get().Entry(this)
            .Collection(s => s.Groups)
            .Query()
            .Include(x => x.Group)
            .LoadAsync();
    }

    public async Task<bool> HasGroup(string group)
    {
        if (Groups is null)
            await LoadUserGroups();

        return Groups!.Any(s => s.Group.Name.ToLower() == group.ToLower());
    }
}