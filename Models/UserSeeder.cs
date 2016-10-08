
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace Firefly.Models{
    public class UserSeeder
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            return;
            var context = serviceProvider.GetService<ApplicationDbContext>();

            string[] roles = new string[] { "Administrator" };

            foreach (string role in roles)
            {
                var roleStore = new RoleStore<Role, ApplicationDbContext, Guid>(context);

                if (!context.Roles.Any(r => r.Name == role))
                {
                    roleStore.CreateAsync(new Role(){Name = role});
                }
            }

            
            var user = new ApplicationUser
            {
                FirstName = "Rudolf",
                LastName = "Dobiáš",
                Email = "ruda.dobias@gmail.com",
                NormalizedEmail = "RUDA.DOBIAS@GMAIL.COM",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                PhoneNumber = "+420604853084",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
            };

            if (!context.Users.Any(u => u.UserName == user.UserName))
            {
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(user,"prdel");
                user.PasswordHash = hashed;

                var userStore = new UserStore<ApplicationUser, Role, ApplicationDbContext, Guid>(context);
                var result = userStore.CreateAsync(user);

            }

            var test = AssignRoles(serviceProvider, user.Email, roles);

            context.SaveChangesAsync();
        }

        public static async Task<IdentityResult> AssignRoles(IServiceProvider services, string email, string[] roles)
        {
            UserManager<ApplicationUser> _userManager = services.GetService<UserManager<ApplicationUser>>();
            ApplicationUser user = await _userManager.FindByEmailAsync(email);
            return await _userManager.AddToRolesAsync(user, roles);
        }

    }

}