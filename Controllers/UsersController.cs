using Firefly.Models;
using Firefly.Providers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        public ApplicationUser Current(){
            //return await manager.GetUserAsync(User);
            return currentUserProvider.GetUser();
        }
    }
}