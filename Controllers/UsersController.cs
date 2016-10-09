using System.Threading.Tasks;
using Firefly.Models;
using Firefly.Providers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System;

namespace Firefly.Controllers{
    
    [Route("/api/users/")]
    public class UsersController : ProtectedController{
        private readonly UserManager<ApplicationUser> manager;
        private readonly ILogger<UsersController> logger;
        private readonly ICurrentUserProvider currentUserProvider;
        public UsersController(UserManager<ApplicationUser> manager, ICurrentUserProvider currentUserProvider, ILogger<UsersController> logger){
            this.manager = manager;
            this.logger = logger;
            this.currentUserProvider = currentUserProvider;
        }
        [HttpGet("current")]
        public async Task<ApplicationUser> Current(){
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return await manager.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}