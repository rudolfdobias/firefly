using System;
using System.Threading.Tasks;
using Firefly.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Firefly.Providers{
    public class CurrentUserProvider : ICurrentUserProvider{
        private ApplicationUser _user;

        private readonly UserManager<ApplicationUser> manager;
        private readonly ILogger<CurrentUserProvider> logger;
        public CurrentUserProvider(UserManager<ApplicationUser> manager, ILogger<CurrentUserProvider> logger){
            this.manager = manager;
            this.logger = logger;
        }

        public CurrentUserProvider SetUser(ApplicationUser User){
            this._user = User;
            return this;
        }

        public ApplicationUser GetUser(){
            return this._user;
        }

        public async void HandleFromHttpContextAsync(HttpContext context){
            if (!context.User.Identity.IsAuthenticated){
                logger.LogInformation("No user found.");
                return;
            }
            this._user = await manager.GetUserAsync(context.User);
            logger.LogInformation("User " + this._user.UserName + " set to global provider.");
        }

        
    }
}