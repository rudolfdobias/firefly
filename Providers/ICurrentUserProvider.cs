using System;
using System.Threading.Tasks;
using Firefly.Models;
using Microsoft.AspNetCore.Http;

namespace Firefly.Providers {
    public interface ICurrentUserProvider{
        CurrentUserProvider SetUser(ApplicationUser User);
        ApplicationUser GetUser();
        Task<ApplicationUser> HandleFromHttpContextAsync(HttpContext context);
        //void HandleRequest(HttpContext context, Func<Task> next);
    }
}