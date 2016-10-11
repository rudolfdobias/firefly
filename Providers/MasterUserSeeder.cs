
using System;
using System.Linq;
using System.Threading.Tasks;
using Firefly.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Firefly.Providers{
    public class MasterUserSeeder : IMasterUserSeeder
    {
        const string DEFAULT_USERNAME = "root";
        const string DEFAULT_ROLE = "Administrator";

        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<MasterUserSeeder> _logger;
        public MasterUserSeeder(ApplicationDbContext DbContext, UserManager<ApplicationUser> UserManager, ILogger<MasterUserSeeder> Logger){
            _dbContext = DbContext;
            _userManager = UserManager;
            _logger = Logger;
        }

        public void Seed() => Seed(DEFAULT_USERNAME, DEFAULT_ROLE);
        public void Seed(string username) => Seed(username, DEFAULT_ROLE);
        public void Seed(string username, string role){
            SeedRole(role);
            SeedUser(username, role);
        }

        private void SeedRole(string name)
        {    
            _logger.LogInformation("Seeding default role " + name + " ...");
            var roleStore = new RoleStore<Role, ApplicationDbContext, Guid>(_dbContext);

            if (!_dbContext.Roles.Any(r => r.Name == name))
            {
                roleStore.CreateAsync(new Role(){Name = name, NormalizedName = name.ToUpper()});
            } else {
                _logger.LogWarning("Skipping - role " + name + " already exist");
            }
        }

        private void SeedUser(string username, string role)
        {
            _logger.LogInformation("Seeding user " + username + " ...");

            var password = Guid.NewGuid().ToString("D");

            var user = new ApplicationUser
            {
                UserName = username,
                NormalizedUserName = username.ToUpper(),
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
            };

            if (!_dbContext.Users.Any(u => u.UserName == username))
            {
                
                var hasher = new PasswordHasher<ApplicationUser>();
                var hashed = hasher.HashPassword(user,password);
                user.PasswordHash = hashed;
                var userStore = new UserStore<ApplicationUser, Role, ApplicationDbContext, Guid>(_dbContext);
                var result = userStore.CreateAsync(user);
                _dbContext.SaveChanges();

            } else {
                _logger.LogWarning("Skipping - user " + username + " already exist");
                return;
            }

            AddUserToRoleAsync(username, role);
            _dbContext.SaveChanges();
            _logger.LogWarning("The Master user was created.\r\n------\r\nUsername: " + username + "\r\nPassword: " +password+"\r\n------\r\n");
        }

        private async void AddUserToRoleAsync(string username, string role)
        {
            _logger.LogInformation("Adding user to role ...");
            ApplicationUser user = await _userManager.FindByNameAsync(username);
            await _userManager.AddToRoleAsync(user, role);
        }

    }

}