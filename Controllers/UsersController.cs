using System.Security.Claims;
using System.Threading.Tasks;
using Firefly.Extensions;
using Firefly.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Firefly.Controllers{
    
    [Route("/api/users/")]
    public class UsersController : ProtectedController{
        private readonly UserManager<ApplicationUser> manager;
        private readonly ILogger<UsersController> logger;
        public UsersController(UserManager<ApplicationUser> manager, ILogger<UsersController> logger){
            this.manager = manager;
            this.logger = logger;
        }
        [HttpGet("current")]
        public async Task<ApplicationUser> Current(){
            logger.LogDebugJson(User);
            return await manager.GetUserAsync(User);
           
        }
    }
}