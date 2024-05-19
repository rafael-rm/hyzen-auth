using Hyzen.SDK.Exception;
using HyzenAuth.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace HyzenAuth.Core.Infrastructure
{
    public class AuthContext : DbContext
    {
        private static AsyncLocal<AuthContext> _instance = new();
        private static readonly object Lock = new();
        
        private AuthContext(string title) { }

        public static AuthContext Get()
        {
            if (_instance.Value == null)
                throw new HException("A transaction cannot be initiated without title.", ExceptionType.InternalError);
            
            return _instance.Value;
        }
		
        public static AuthContext Get(string title)
        {
            if (_instance.Value != null)
                throw new HException($"A transaction can only have one title {title}.", ExceptionType.InternalError);
            
            return _instance.Value = new AuthContext(title);
        }

        public DbSet<User> UsersSet { get; set; }
        public DbSet<Role> RolesSet { get; set; }
        public DbSet<UserRole> UsersRolesSet { get; set; }
        public DbSet<Group> GroupsSet { get; set; }
        public DbSet<GroupRole> GroupsRolesSet { get; set; }
        public DbSet<UserGroup> UsersGroupsSet { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
                return;

            lock (Lock)
            {
                if (optionsBuilder.IsConfigured)
                    return;

                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .AddUserSecrets<Program>()
                    .Build();

                var connectionString = string.IsNullOrEmpty(configuration.GetConnectionString("DefaultConnection"))
                    ? Environment.GetEnvironmentVariable("HYZEN_DATABASE_ALFA", EnvironmentVariableTarget.User)
                    : configuration.GetConnectionString("DefaultConnection");

                optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 28)));
            }
        }
        
        public override void Dispose()
        {
            base.Dispose();
            _instance.Value = null;
        }
		
        public override async ValueTask DisposeAsync()
        {
            await base.DisposeAsync();
            _instance.Value = null;
        }
    }
}