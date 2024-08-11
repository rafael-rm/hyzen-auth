using System.Diagnostics;
using Auth.Core.Models;
using Hyzen.SDK.Exception;
using Hyzen.SDK.SecretManager;
using Microsoft.EntityFrameworkCore;

namespace Auth.Core.Infrastructure
{
    public class AuthContext : DbContext
    {
        private static readonly AsyncLocal<AuthContext> Instance = new();
        private static readonly object Lock = new();
        
        public DbSet<User> UsersSet { get; set; }
        public DbSet<Role> RolesSet { get; set; }
        public DbSet<UserRole> UsersRolesSet { get; set; }
        public DbSet<Group> GroupsSet { get; set; }
        public DbSet<GroupRole> GroupsRolesSet { get; set; }
        public DbSet<UserGroup> UsersGroupsSet { get; set; }
        
        private AuthContext(string title) { }

        public static AuthContext Get()
        {
            if (Instance.Value == null)
                throw new HException("No transaction was initiated in this context", ExceptionType.InternalError);
            
            return Instance.Value;
        }
		
        public static AuthContext Get(string name)
        {
            if (Instance.Value != null)
                throw new HException("A transaction has already been started in this context.", ExceptionType.InternalError);
            
            return Instance.Value = new AuthContext(name);
        }
        
        public static void Reset()
        {
            Instance.Value = null;
        }

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

                var connectionString = HyzenSecret.GetSecret("CS-HYZEN-DATABASE-ALFA");
                
                if (Debugger.IsAttached && string.IsNullOrWhiteSpace(connectionString))
                {
                    connectionString = configuration.GetConnectionString("DefaultConnection");
                }

                optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 28)));
            }
        }
        
        public override void Dispose()
        {
            base.Dispose();
            Instance.Value = null;
        }
		
        public override async ValueTask DisposeAsync()
        {
            await base.DisposeAsync();
            Instance.Value = null;
        }
    }
}